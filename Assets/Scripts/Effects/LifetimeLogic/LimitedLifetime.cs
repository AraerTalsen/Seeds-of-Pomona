using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LimitedLifetime : LifetimeLogic
{
    [SerializeField] private float lifespan;
    public float Lifespan => lifespan;
    private float startTime;
    public override bool ShouldDestroy() => startTime + Lifespan < Time.time;
}
