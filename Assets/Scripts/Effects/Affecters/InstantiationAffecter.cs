using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InstantiationAffecter : Affecter<InstantiationAffecter>
{
    public enum ParentSelect { none, host, target }
    [SerializeField] private bool isProjectile;
    [SerializeField] private bool isSpawnAtOrigin;
    [SerializeField] private float speed;
    [SerializeField] private float nodeDuration;
    [SerializeField] private GameObject node;
    [SerializeField] private ParentSelect parentSelection;

    public bool IsProjectile => isProjectile;
    public bool IsSpawnAtOrigin => isSpawnAtOrigin;
    public float Speed => speed;
    public GameObject Node => node;
    public ParentSelect ParentSelection => parentSelection;
    public float NodeDuration => nodeDuration;

    public InstantiationAffecter(EffectParameters parameters)
    {
        (
            lockMovement, stackableUntil,
            lifetimeLabel, lifetime,
            repeatLabel, runs,
            targetLabel, coordinator
        ) = parameters;    
    }

    public InstantiationAffecter(Affecter affecter = null)
    {
        if(affecter != null) 
        {
            (
                lockMovement, stackableUntil,
                lifetimeLabel, lifetime,
                repeatLabel, runs,
                targetLabel, coordinator
            ) = affecter;
        }
    }

    public override Task<IRuntimeEvent> CreateRuntimeEvent(EffectContext context, Action<EffectContext> callback = null)
    {
        RegisterRulebook(InstantiationEffectRulebook.Instance);
        return base.CreateRuntimeEvent(context, callback);
    }
}
