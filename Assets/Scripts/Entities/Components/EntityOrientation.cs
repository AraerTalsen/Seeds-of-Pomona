using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityOrientation : MonoBehaviour
{
    protected Vector2 currentOrientation;
    public virtual Vector2 CurrentOrientation { get => currentOrientation; set => currentOrientation = value.normalized; }
}
