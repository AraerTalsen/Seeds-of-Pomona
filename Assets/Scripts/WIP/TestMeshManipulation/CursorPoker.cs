using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorPoker : MonoBehaviour
{
    public GameObject pointer, vertex;
    private float distFromCam = 0.75f;
    private bool visualBuilt = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AttemptPoke();
    }

    private void AttemptPoke()
    {
        //if (Input.GetMouseButton(0))
        //{
            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit);
            MovePointer(hit.point);
            if(hit.collider != null)
                PokeMesh(hit.collider.gameObject, hit.point);
        //}
    }

    private void PokeMesh(GameObject pokedObj, Vector3 pokePoint)
    {
        MeshFilter meshFilter = pokedObj.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        int[] triangles = mesh.triangles;
        Vector3[] normals = mesh.normals;
        Vector3 [] vertices = mesh.vertices;
        Vector2[] uvs = mesh.uv;

        if(!visualBuilt) BuildMeshVisual(pokedObj.transform.position, triangles, vertices);
        print(pokedObj.transform.InverseTransformPoint(pokePoint));
    }

    private void MovePointer(Vector3 targetPoint)
    {
        pointer.transform.position = targetPoint;
    }

    private void BuildMeshVisual(Vector3 objPos, int[] triangles, Vector3[] vertices)
    {
        visualBuilt = true;
        VisualizeMeshPoints(objPos, vertices);
        VisualizeMeshTriangles(objPos, triangles, vertices);
    }

    private void VisualizeMeshPoints(Vector3 objPos, Vector3[] vertices)
    {
        foreach(Vector3 v in vertices)
        {
            Instantiate(vertex, v + objPos, Quaternion.identity);
        }
    }

    private void VisualizeMeshTriangles(Vector3 objPos, int[] triangles, Vector3[] vertices)
    {
        for(int j = 0; j < triangles.Length; j += 3)
        {
            Vector3 point1 = vertices[triangles[j]] + objPos;
            Vector3 point2 = vertices[triangles[j + 1]] + objPos;
            Vector3 point3 = vertices[triangles[j + 2]] + objPos;

            Debug.DrawLine(point1, point2, Color.blue, Mathf.Infinity);
            Debug.DrawLine(point2, point3, Color.blue, Mathf.Infinity);
            Debug.DrawLine(point3, point1, Color.blue, Mathf.Infinity);
        }
    }
}
