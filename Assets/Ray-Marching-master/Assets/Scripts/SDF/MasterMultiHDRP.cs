using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/MasterMultiHDRP")]
public sealed class MasterMultiHDRP : CustomPostProcessVolumeComponent, IPostProcessComponent
{

  public ClampedFloatParameter depthDistance = new ClampedFloatParameter(1f, 0f, 32f);
  Material m_Material;
  public ComputeShader raymarching;
  Camera cam;
  RenderTexture target;
  MaterialPropertyBlock matProperties;
  RTHandle rth1;
  RTHandle rth2;
  // RenderTexture depthTexture;
  public float nearPlaneDepth;
  public float farPlaneDepth;
  [SerializeField]
  public Light[] lightSources;
  List<ComputeBuffer> buffersToDispose;
  List<RTHandle> rtHandles;



  public bool IsActive() => m_Material != null;
  public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

  public override void Setup()
  {

    if (Shader.Find("Hidden/Shader/DepthExample") != null)
    {
      m_Material = new Material(Shader.Find("Hidden/Shader/DepthExample"));
    }

    matProperties = new MaterialPropertyBlock();

  }


  public RTHandle GetNewRTHandle(HDCamera camera)
  {
      var width = camera.actualWidth;
      var height = camera.actualHeight;
      const GraphicsFormat RTFormat = GraphicsFormat.R16G16B16A16_SFloat;
      var rt = RTHandles.Alloc(scaleFactor: Vector2.one, colorFormat: RTFormat);// RTHandles.Alloc(width, height,  colorFormat: RTFormat);

      // rtHandles.Add(rt);
      return rt;
  }

  void Init () {
    // cam = Camera.current;
    // cam = GetComponent<Camera>();
    // cam.depthTextureMode = DepthTextureMode.Depth;
    // depthTexture = RenderTexture.GetTemporary(cam.pixelWidth, cam.pixelHeight, 24);
    // lightSource = FindObjectOfType<Light> ();

  }


  public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
  {

    //mat is the material which contains the shader
    //we are passing the destination RenderTexture to

    if (m_Material == null)
    {
      // Debug.Log("material stuck at null");
      return;
    }

    if (rth1 == null)
      rth1 = GetNewRTHandle(camera);
    if (rth2 == null)
      rth2 = GetNewRTHandle(camera);

    // Init ();
    buffersToDispose = new List<ComputeBuffer> ();

    // InitRenderTexture ();
    // CreateScene ();
    // SetParameters ();

    // get depth texture
    Vector4 parameters = new Vector4(depthDistance.value, depthDistance.value, depthDistance.value, depthDistance.value);

    m_Material.SetVector("_Params", parameters);
    m_Material.SetTexture("_InputTexture", source);
    HDUtils.DrawFullScreen(cmd, m_Material, rth1, matProperties, 3);

    // // RenderTexture sourceCopy = source;
    // HDUtils.DrawFullScreen(cmd, m_Material, rth1);
    //
    // // Graphics.Blit(source,depthTexture);
    //
    raymarching.SetTexture (0, "Depth", source.rt);
    raymarching.SetTexture (0, "Source", source);
    raymarching.SetTexture (0, "Destination", rth1);

    raymarching.SetFloat ("nearPlane", nearPlaneDepth);
    raymarching.SetFloat ("farPlane", farPlaneDepth);

    int threadGroupsX = Mathf.CeilToInt (cam.pixelWidth / 8.0f);
    int threadGroupsY = Mathf.CeilToInt (cam.pixelHeight / 8.0f);
    int kernelHandle = raymarching.FindKernel("CSMain");
    raymarching.Dispatch (kernelHandle, threadGroupsX, threadGroupsY, 1);
    // Graphics.Blit(target, destination);

    m_Material.SetTexture("_InputTexture", rth1);
    HDUtils.DrawFullScreen(cmd, m_Material, destination);
    // cam.targetTexture = depthTexture;
    // mask things

    // RenderTexture.ReleaseTemporary(depthTexture);
    foreach (var buffer in buffersToDispose) {
      buffer.Dispose ();
    }
  }

  public override void Cleanup() => CoreUtils.Destroy(m_Material);
  // public override void Cleanup() => CoreUtils.Destroy(m_Material);

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
      if (target == null || target.width != cam.pixelWidth || target.height != cam.pixelHeight) {
        if (target != null) {
          target.Release ();
        }
        target = new RenderTexture (cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
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