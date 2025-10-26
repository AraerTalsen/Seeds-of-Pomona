using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Track the neighbors (adjacent verteces) of each vertex in a mesh
public class SetVertexNeighborsForMesh : MonoBehaviour
{
    public static Dictionary<Vector3, VertexObject> SetVertexNeighbors(Mesh m)
    {
        Dictionary<Vector3, VertexObject> vertices = new Dictionary<Vector3, VertexObject>();
        int[] tris = m.triangles;
        Vector3[] verts = m.vertices;
        for(int i = 0; i < tris.Length; i += 3)
        {
            int[] vertIndeces = {tris[i], tris[i + 1], tris[i + 2]};
            Vector3[] vCoords = {verts[vertIndeces[0]], verts[vertIndeces[1]], verts[vertIndeces[2]]};
            VertexObject vObj;
            for(int j = 0; j < vertIndeces.Length; j++)
            {
                if(vertices.ContainsKey(vCoords[j]))
                    vObj = vertices[vCoords[j]];
                else 
                {
                    vObj = new VertexObject(vCoords[j]);
                    vertices.Add(vCoords[j], vObj);
                }
                vObj.AddNeighbors(vCoords.Where((v, index) => index != j && (vObj.Neighbors == null || !vObj.Neighbors.Contains(v))).ToArray());
            }
        }
        return vertices;
    }
}
