using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformAffector : Affector
{
    public enum TransformLabel { position, rotation, scale }
    [SerializeField] private TransformLabel transformLabel;
    [SerializeField] private bool isInstant = true;
    [SerializeField] private float speed;
    [SerializeField] private bool useHostStat;
    [SerializeField] private Stats selectedStat;
    [SerializeField] private bool hasTarget = true;
    [SerializeField] private Vector2 position;
    [SerializeField] private float angle;
    [SerializeField] private float scale;
    [SerializeField] private Vector2 direction;
    [SerializeField] private bool isClockwise;
    
    public TransformAffector(Affector affector = null)
    {
        if(affector != null) (lifetimeLabel, lifetime, repeatLabel, runs, targetLabel, coordinator) = affector;
    }

    public TransformLabel TransformType => transformLabel;
    public bool IsInstant => isInstant;
    public float Speed => speed;
    public bool UseHostStat => useHostStat;
    public Stats SelectedStat => selectedStat;
    public bool HasTarget => hasTarget;
    public Vector2 Position => position;
    public float Angle => angle;
    public float Scale => scale;
    public Vector2 Direction => direction;
    public bool IsClockwise => isClockwise;

    public override IRuntimeEvent CreateRuntimeEvent(EffectContext context)
    {
        return new RuntimeEvent<TransformAffector>
        (
            context,
            this,
            TransformEffectRulebook.GetCurrentEffect(this),
            CallbackWrapper.WrapAction(TogglePauseEntity)
        );
    }
}
