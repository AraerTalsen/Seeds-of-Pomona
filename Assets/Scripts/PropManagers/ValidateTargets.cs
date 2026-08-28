using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateTargets : MonoBehaviour
{
    [SerializeField] private Sprite impact;
    private TargetAreaManager targetAreaManager;
    private SpriteRenderer sr;

    private void Start()
    {
        
        targetAreaManager = transform.parent.GetComponent<TargetAreaManager>();
        sr = GetComponent<SpriteRenderer>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject g = other.gameObject;
        if(!targetAreaManager.Targets.Contains(g))
            targetAreaManager.Targets.Add(g);
        
        sr.sprite = impact;
    }
}
