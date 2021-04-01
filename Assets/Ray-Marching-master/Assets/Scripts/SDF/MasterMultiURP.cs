using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using System;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class MasterMultiURP : MonoBehaviour {
    public ComputeShader raymarching;

    RenderTexture target;
    public RenderTexture depthTexture;
    public RenderTexture outputTexture;
    Camera cam;

    private int textureWidth;
    private int textureHeight;
    // public Material depthMaterial;
    public float depthScaler;
    // public float farPlaneDepth;
    [SerializeField]
    public Light[] lightSources;
    List<ComputeBuffer> buffersToDispose;

    void Init () {
        // cam = Camera.current;
        cam = GetComponent<Camera>();
        cam.aspect = 16f/9f;
        textureWidth = outputTexture.width;
        textureHeight = outputTexture.height;

        // outputTexture.enableRandomWrite = true;
        // inputTexture.enableRandomWrite = true;
        // cam.depthTextureMode = DepthTextureMode.Depth;
        // lightSource = FindObjectOfType<Light> ();
    }

    // void Start()
    // {
    //   Init();
    // }


    void LateUpdate () {
      //mat is the material which contains the shader
      //we are passing the destination RenderTexture to
      // return;

        Init ();
        buffersToDispose = new List<ComputeBuffer> ();

        InitRenderTexture ();
        CreateScene ();
        SetParameters ();

        ///////////////depthTexture = RenderTexture.GetTemporary(cam.pixelWidth, cam.pixelHeight, 24);

        // Create a Material that uses the DepthCopy Shader
        ///////////////Material m_DepthCopyMat = new Material(Shader.Find("depthShaderURP"));

        // Set the _MyDepthTex Shader Texture to our source depth texture to be copied
        // m_DepthCopyMat.SetTexture("_MyDepthTex", source);

        // Do a Blit using the DepthCopy Material/Shader
        // Graphics.Blit(source, depthTexture, m_DepthCopyMat);

        // get depth texture
        // RenderTexture sourceCopy = source;

        // Graphics.Blit(source, depthTexture, depthMaterial);
        raymarching.SetFloat("Width", textureWidth);
        raymarching.SetFloat("Height", textureHeight);

        raymarching.SetTexture (0, "Depth", depthTexture);
        // raymarching.SetTexture (0, "Source", inputTexture);
        raymarching.SetTexture (0, "Destination", target);
        // raymarching.SetTexture (0, "Destination", target);

        raymarching.SetFloat ("depthScaler", depthScaler);
        // raymarching.SetFloat ("farPlane", farPlaneDepth);

        int threadGroupsX = Mathf.CeilToInt (textureWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt (textureHeight / 8.0f);
        int kernelHandle = raymarching.FindKernel("CSMain");
        raymarching.Dispatch (kernelHandle, threadGroupsX, threadGroupsY, 1);
        Graphics.Blit(target, outputTexture);

        // cam.targetTexture = destination;
        // mask things

        ///////////////RenderTexture.ReleaseTemporary(depthTexture);
        foreach (var buffer in buffersToDispose) {
            buffer.Dispose ();
        }
    }

    void CreateScene () {
        List<Shape> allShapes = new List<Shape> (FindObjectsOfType<Shape> ());
        allShapes.Sort ((a, b) => a.operation.CompareTo (b.operation));

        List<Shape> orderedShapes = new List<Shape> ();

        for (int i = 0; i < allShapes.Count; i++) {
            // Add top-level shapes (those without a parent)
            if (allShapes[i].transform.parent == null) {

                Transform parentShape = allShapes[i].transform;
                orderedShapes.Add (allShapes[i]);
                allShapes[i].numChildren = parentShape.childCount;
                // Add all children of the shape (nested children not supported currently)
                for (int j = 0; j < parentShape.childCount; j++) {
                    if (parentShape.GetChild (j).GetComponent<Shape> () != null) {
                        orderedShapes.Add (parentShape.GetChild (j).GetComponent<Shape> ());
                        orderedShapes[orderedShapes.Count - 1].numChildren = 0;
                    }
                }
            }

        }

        ShapeData[] shapeData = new ShapeData[orderedShapes.Count];
        for (int i = 0; i < orderedShapes.Count; i++) {
            var s = orderedShapes[i];
            Vector3 col = new Vector3 (s.colour.r, s.colour.g, s.colour.b);
            shapeData[i] = new ShapeData () {
                position = s.Position,
                scale = s.Scale, colour = col,
                shapeType = (int) s.shapeType,
                operation = (int) s.operation,
                blendStrength = s.blendStrength*3,
                numChildren = s.numChildren
            };
        }

        int kernelHandle = raymarching.FindKernel("CSMain");
        ComputeBuffer shapeBuffer = new ComputeBuffer (shapeData.Length, ShapeData.GetSize ());
        shapeBuffer.SetData (shapeData);
        raymarching.SetBuffer (kernelHandle, "shapes", shapeBuffer);
        raymarching.SetInt ("numShapes", shapeData.Length);

        buffersToDispose.Add (shapeBuffer);


        // do lights
        LightData[] lightData = new LightData[lightSources.Length];
        // get positions of lights
        for (int i = 0; i < lightSources.Length; i ++) {
          lightData[i] = new LightData() {position = lightSources[i].transform.position,
                                          pointLight = lightSources[i].type != LightType.Directional ? 1 : 0,
                                          scale = Vector3.one};
        }
        ComputeBuffer lightBuffer = new ComputeBuffer (lightData.Length, LightData.GetSize ());
        lightBuffer.SetData(lightData);
        raymarching.SetBuffer (kernelHandle, "lights", lightBuffer);
        raymarching.SetInt ("numLights", lightData.Length);
        buffersToDispose.Add (lightBuffer);

        // do depth texture
        // Get (update only if necessary, check for null)
        // RenderTexture depth = Shader.GetGlobalTexture( "_CameraDepthTexture" ) as RenderTexture;
        // Texture gBuffer2 = Shader.GetGlobalTexture( "_CameraGBufferTexture2" );

        // Set
        // int kernelHandle = raymarching.FindKernel("CSMain");
        // raymarching.SetTextureFromGlobal(kernelHandle, Shader.PropertyToID("_CameraDepthTexture"), Shader.PropertyToID("_CameraDepthTexture"));
        // raymarching.SetTexture( 0, "_CameraGBufferTexture2", gBuffer2 );

        // raymarching.SetTextureFromGlobal(0, "_DepthTexture", "_CameraDepthTexture");


    }

    void SetParameters () {
        // bool lightIsDirectional = lightSources.type == LightType.Directional;
        raymarching.SetMatrix ("_CameraToWorld", cam.cameraToWorldMatrix);
        raymarching.SetMatrix ("_CameraInverseProjection", cam.projectionMatrix.inverse);
        // raymarching.SetVector ("Lights", (lightIsDirectional) ? lightSources.transform.forward : lightSources.transform.position);

    }

    void InitRenderTexture () {
        if (target == null || target.width != textureWidth || target.height != textureHeight) {
            if (target != null) {
                target.Release ();
            }
            target = new RenderTexture (textureWidth, textureHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create ();
        }

    }

    struct ShapeData {
        public Vector3 position;
        public Vector3 scale;
        public Vector3 colour;
        public int shapeType;
        public int operation;
        public float blendStrength;
        public int numChildren;

        public static int GetSize () {
            return sizeof (float) * 10 + sizeof (int) * 3;
        }
    }
    struct LightData {
        public Vector3 position;
        public Vector3 scale;
        public int pointLight;
        // public bool fillerOne;
        // public bool fillerTwo;
        // public bool fillerThree;

        public static int GetSize () {
            return sizeof (float) * 6 + sizeof (bool) * 4;
        }
    }
}
