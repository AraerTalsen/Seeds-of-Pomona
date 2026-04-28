using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RuntimeEvent<T> : IRuntimeEvent where T : IRuntimeFactory
{
    private readonly EffectContext context;
    private readonly T affector;
    private readonly LifetimeLogic lifetime;
    private Action<EffectContext, T> tickProc;
    private Action<EffectContext> callback;
    public bool IsFinished
    {
        get
        {
            bool isFinished = lifetime.ShouldDestroy(context);
            if(isFinished) callback?.Invoke(context);
            return isFinished;
        }
    }

    public RuntimeEvent(EffectContext context, T affector, Action<EffectContext, T> tickProc, Action<EffectContext> callback = null)
    {
        this.context = context;
        this.affector = affector;
        this.tickProc = tickProc;
        this.callback = callback;
        lifetime = affector.Lifetime.Activate();

        this.affector.Apply(context);
    }

    public void Tick() => tickProc(context, affector);
}
