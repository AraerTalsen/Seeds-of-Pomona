using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomMeshGenerator : MonoBehaviour
{
    [SerializeField]
    private Sprite Sprite;

    void Start()
    {
        GenerateNewMesh();
    }

    public void GenerateNewMesh(Sprite s = null)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        Mesh m = s != null ? SpriteToMesh(s) : new Mesh();

        meshFilter.mesh = m;
        meshCollider.sharedMesh = m;
    }

    private Mesh SpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        
        mesh.SetVertices(Array.ConvertAll(sprite.vertices, i => (Vector3)i).ToList());
        mesh.SetUVs(0, sprite.uv.ToList());
        Vector3[] normals = new Vector3[mesh.vertices.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.back; // Normal points backward for a 2D object facing the camera.
        }
        mesh.normals = normals;
        mesh.SetTriangles(Array.ConvertAll(sprite.triangles, i => (int)i), 0);

        return mesh;
    }
}
