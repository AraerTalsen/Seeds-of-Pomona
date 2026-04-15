using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetCoordinator
{
    [SerializeField] private bool hasBounds;
    [SerializeField] private bool isDynamic;
    [SerializeField] private GameObject area;

    public Vector2 GetOrigin() => !isDynamic ? Vector2.zero/*when context is added, use host pos*/ : MousePos;
    private Vector2 MousePos => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
}
