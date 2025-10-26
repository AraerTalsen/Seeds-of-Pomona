using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

//Initialize plant mesh to the shape of sprite and assign root position
//Consider extending from CustomMeshGenerator
public class InitializeCustomMesh : MonoBehaviour
{
    public Sprite s;
    public Transform rootPos;
    public GameObject point;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        Mesh m = SpriteToMesh(s);

        meshFilter.mesh = m;
        meshCollider.sharedMesh = m;
        Dictionary<Vector3, VertexObject> vDic = SetVertexNeighborsForMesh.SetVertexNeighbors(m);
        GetComponent<Sliceable>().VertexNetwork = vDic;

        if (rootPos != null)
        {
            Sliceable sliceable = GetComponent<Sliceable>();
            sliceable.RootPos = rootPos.localPosition;
        }
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
