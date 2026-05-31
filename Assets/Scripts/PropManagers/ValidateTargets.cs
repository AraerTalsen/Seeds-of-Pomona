using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateTargets : MonoBehaviour
{
    private TargetAreaManager targetAreaManager;

    private void Start()
    {
        targetAreaManager = transform.parent.GetComponent<TargetAreaManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject g = other.gameObject;
        if(!targetAreaManager.Targets.Contains(g))
            targetAreaManager.Targets.Add(g);
    }
}
