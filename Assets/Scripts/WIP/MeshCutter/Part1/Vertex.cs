using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public Vector3 point;
    public string name; 
    public Vertex(Vector3 vPoint, string vName)
    {
        point = vPoint;
        name = vName;
    }
}
