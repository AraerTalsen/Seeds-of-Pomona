using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TransformAffecter : Affecter<TransformAffecter>
{
    public enum TransformLabel { position, rotation, scale }
    [SerializeField] private TransformLabel transformLabel;
    [SerializeField] private bool isInstant = true;
    [SerializeField] private bool isKinematic = true;
    [SerializeField] private float speed;
    [SerializeField] private bool useHostStat;
    [SerializeField] private Stats selectedStat;
    [SerializeField] private bool hasTarget = true;
    [SerializeField] private Vector2 position;
    [SerializeField] private float angle;
    [SerializeField] private float scale;
    [SerializeField] private Vector2 direction;
    [SerializeField] private bool isOptimal;
    [SerializeField] private bool isClockwise;

    public TransformLabel TransformType => transformLabel;
    public bool IsInstant => isInstant;
    public bool IsKinematic => isKinematic;
    public float Speed => speed;
    public bool UseHostStat => useHostStat;
    public Stats SelectedStat => selectedStat;
    public bool HasTarget => hasTarget;
    public Vector2 Position => position;
    public float Angle => angle;
    public float Scale => scale;
    public Vector2 Direction => direction;
    public bool IsOptimal => isOptimal;
    public bool IsClockwise => isClockwise;
    
    public TransformAffecter(EffectParameters parameters)
    {
        (
            lockMovement, stackableUntil,
            lifetimeLabel, lifetime,
            repeatLabel, runs,
            targetLabel, coordinator
        ) = parameters;    
    }

    public TransformAffecter(Affecter affecter = null)
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
        RegisterRulebook(TransformEffectRulebook.Instance);
        return base.CreateRuntimeEvent(context, callback);
    }
}
