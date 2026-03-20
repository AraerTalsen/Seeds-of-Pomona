using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An object that stretches out along the virtual xy plane 
//(in 2D iso-space, this is the 45 degree angles between the x and y axes)
//needs to having a sorting gradient instead of a pivot so that the illusion of depth isn't
//broken when an entity walks along its perimeter
public struct SortingGradient
{
    public Vector2 NearPoint { get; set; }
    public Vector2 FarPoint { get; set; }
    
    public SortingGradient(Vector2 nearPoint, Vector2 farPoint)
    {
        NearPoint = nearPoint;
        FarPoint = farPoint;
    }
}
