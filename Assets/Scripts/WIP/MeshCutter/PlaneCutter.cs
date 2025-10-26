using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using System;

public class PlaneCutter : MonoBehaviour
{     
    private Vector3 start, end;
    public List<GameObject> sliceObjects = new List<GameObject>();
    public GameObject[] pointTypes;

    private GameObject[] arr;
    private Vector3 n1;
    private List<Vector3> intersectingPoints;
    private bool draw = false;
    private float distFromCam = 1.75f;

    //Select objects under line between two points submitted by mouse left button down and release positions
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            sliceObjects.Clear();
            Vector3 mousePos = Input.mousePosition;
            start = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distFromCam));
        }
        else if(Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 current = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distFromCam));
            Debug.DrawRay(start, current - start, Color.red);

            Vector3 dir = (Vector3.forward - current).normalized;

            if(Physics.Raycast(current, dir, out RaycastHit hit, Mathf.Infinity))
            {
                if(sliceObjects.Count > 0 && !sliceObjects.Find( g => g == hit.transform.gameObject))
                    sliceObjects.Add(hit.transform.gameObject);
                else if (sliceObjects.Count == 0)
                    sliceObjects.Add(hit.transform.gameObject);
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos = Input.mousePosition;
            end = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distFromCam));

            SliceObjectsInList();
        }

        if(draw)
        {
            DrawLinesForTriangles();
            DrawCut();
        }

        if(Input.GetKeyUp(KeyCode.Space))
            CompleteSlice();
    }

    //Create plane along line and iterate through selection to use Slicer script on each object
    private void SliceObjectsInList()
    {
        foreach(GameObject g in sliceObjects)
        {
            Vector3 _triggerEnterTipPosition = start;
            Vector3 _triggerEnterBasePosition = end;
            Vector3 _triggerExitTipPosition = start + Vector3.forward;

            //Create a triangle between the tip and base so that we can get the normal
            Vector3 side1 = _triggerExitTipPosition - _triggerEnterTipPosition;
            Vector3 side2 = _triggerExitTipPosition - _triggerEnterBasePosition;

            //Get the point perpendicular to the triangle above which is the normal
            //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
            Vector3 normal = Vector3.Cross(side1, side2).normalized;

            //Transform the normal so that it is aligned with the object we are slicing's transform.
            Vector3 transformedNormal = ((Vector3)(g.transform.localToWorldMatrix.transpose * normal)).normalized;

            //Get the enter position relative to the object we're cutting's local transform
            Vector3 transformedStartingPoint = g.transform.InverseTransformPoint(_triggerEnterTipPosition);

            Plane plane = new Plane();

            plane.SetNormalAndPosition(
                    transformedNormal,
                    transformedStartingPoint);

            var direction = Vector3.Dot(Vector3.up, transformedNormal);

            //Flip the plane so that we always know which side the positive mesh is on
            if (direction < 0)
            {
                plane = plane.flipped;
            }
            
            Vector3[] planeConstraint = {start, end, new Vector3(0, 1, -8.25f)};
            GameObject[] slices = Slicer.Slice(plane, g, planeConstraint, out intersectingPoints);
            arr = new GameObject[]{g, slices[0], slices[1]};
            arr[2].GetComponent<Rigidbody>().useGravity = false;
            arr[2].GetComponent<Rigidbody>().isKinematic = true;
            PlaceVertexPoints(g, arr);
            //PlaceUVPoints(g, arr);

            n1 = transformedNormal;
            draw = true;
        }
    }

    //Debugger for seeing position of each vertex of the original shape and subsequent halves. Comment out CompleteSlice() function call to use.
    private void PlaceVertexPoints(GameObject original, GameObject[] arr)
    {
        for(int i = 1; i < arr.Length; i++)
        {
            Vector3[] vertices = arr[i].GetComponent<MeshFilter>().mesh.vertices;
            Vector3 mod = i == 0 ? Vector3.zero : Vector3.left;
            for(int j = 0; j < vertices.Length; j++)
            {
                Vector3 v = vertices[j];
                GameObject type = intersectingPoints.Contains(v) ? pointTypes[3] : pointTypes[i];
                GameObject g = Instantiate(type);
                g.transform.parent = original.transform;
                g.transform.localPosition = v;
            }
        }
    }

    //Debugger for seeing position of each UV of the original shape and subsequent halves. Comment out CompleteSlice() function call to use.
    private void PlaceUVPoints(GameObject original, GameObject[] arr)
    {
        for(int i = 1; i < arr.Length; i++)
        {
            Vector2[] uvs = arr[i].GetComponent<MeshFilter>().mesh.uv;
            Vector3 mod = i == 0 ? Vector3.zero : Vector3.left * 1.5f;
            for(int j = 0; j < uvs.Length; j++)
            {
                Vector3 v = uvs[j];
                GameObject type = intersectingPoints.Contains(v) ? pointTypes[3] : pointTypes[i];
                GameObject g = Instantiate(type);
                g.transform.position = v + mod + new Vector3(0, 0.5f, -8);
                g.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }
    }

    //Debugger for seeing the triangles of the original shape and subsequent halves. Comment out CompleteSlice() function call to use.
    private void DrawLinesForTriangles()
    {
        for(int i = 0; i < arr.Length; i++)
        {
            Mesh m = arr[i].GetComponent<MeshFilter>().mesh;
            int[] triangles = m.triangles;
            Vector3[] vertices = m.vertices;
            Vector3 mod = i == 0 ? Vector3.zero : Vector3.left;
            for(int j = 0; j < triangles.Length; j += 3)
            {
                Vector3 localize = new Vector3(0, 1, -8);
                Vector3 point1 = vertices[triangles[j]] + localize + mod;
                Vector3 point2 = vertices[triangles[j + 1]] + localize + mod;
                Vector3 point3 = vertices[triangles[j + 2]] + localize + mod;

                Debug.DrawLine(point1, point2, Color.blue);
                Debug.DrawLine(point2, point3, Color.blue);
                Debug.DrawLine(point3, point1, Color.blue);
            }
        }
    }

    //Debugger for seeing where the slicing line was drawn. Comment out CompleteSlice() function call to use.
    private void DrawCut()
    {
        Debug.DrawLine(start, end, Color.red);
        Debug.DrawLine(start + Vector3.left, end + Vector3.left, Color.red);
    }

    //Compartmentalized for easy debugging
    private void CompleteSlice()
    {
        Destroy(arr[0]);

        Rigidbody rigidbody = arr[2].GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        Vector3 newNormal = n1 + Vector3.up * 2;
        rigidbody.AddForce(newNormal, ForceMode.Impulse);
        draw = false;
    }
}
