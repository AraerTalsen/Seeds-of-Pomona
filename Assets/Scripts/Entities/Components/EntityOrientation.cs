using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityOrientation : MonoBehaviour
{
    private Vector2 currentOrientation;
    public Vector2 CurrentOrientation { get => currentOrientation; set => currentOrientation = value.normalized; }
}
