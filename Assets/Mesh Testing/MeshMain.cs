using System.Collections;
using System.Collections.Generic;
using Parabox.CSG;
using UnityEngine;

public class MeshMain : MonoBehaviour
{
    // public GameObject baseTarget;
    // private MeshFilter mf;
    // private MeshRenderer mr;
    private Mesh boolMesh;

    // mesh coords
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;

    void InitMesh()
    {
      // :)
    }

    void UpdateMesh(Mesh newMesh)
    {
      if (boolMesh != null && newMesh != null)
      {
        boolMesh.Clear();

        vertices = newMesh.vertices;
        triangles = newMesh.triangles;
        uv = newMesh.uv;

        for (int i = 0; i < vertices.Length; i++) {
          vertices[i] -= transform.position;
        }

        boolMesh.vertices = vertices;
        boolMesh.uv = uv;
        boolMesh.triangles = triangles;
        boolMesh.RecalculateNormals();
      }
    }

    // Start is called before the first frame update
    void Start()
    {

      boolMesh = GetComponent<MeshFilter>().mesh;
      boolMesh.name = "boolMesh";

    }

    void OnCollisionEnter(Collision col)
    {

      CSG_Model result = Boolean.Subtract(gameObject, col.gameObject);

      UpdateMesh(result.mesh);

      Debug.Log("mesh updated");

    }

    // // Update is called once per frame
    // void Update()
    // {
    //
    // }
}
