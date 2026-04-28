using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRuntimeFactory
{
    public LifetimeLogic Lifetime { get; }
    public void Apply(EffectContext context);
    public IRuntimeEvent CreateRuntimeEvent(EffectContext context);
}
