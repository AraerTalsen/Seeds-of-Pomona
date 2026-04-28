using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LimitedLifetime : LifetimeLogic
{
    [SerializeField] private float lifespan;
    public float Lifespan => lifespan;
    public float StartTime { get; set; }

    public LimitedLifetime(float lifespan = 0.0f) { this.lifespan = lifespan; }

    public override bool ShouldDestroy(EffectContext context) => StartTime + Lifespan < Time.time;

    public override LifetimeLogic Activate()
    {
        LimitedLifetime activated = (LimitedLifetime)Clone();
        activated.StartTime = Time.time;
        return activated;
    }

    public override LifetimeLogic Clone() => new LimitedLifetime(lifespan);
}
