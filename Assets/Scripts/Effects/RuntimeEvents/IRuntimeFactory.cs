using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IRuntimeFactory
{
    public LifetimeLogic Lifetime { get; }
    public Task Apply(EffectContext context);
    public Task<IRuntimeEvent> CreateRuntimeEvent(EffectContext context, Action<EffectContext> callback = null);
}
