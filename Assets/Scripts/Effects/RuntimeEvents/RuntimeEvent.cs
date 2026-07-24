using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class RuntimeEvent<T> : IRuntimeEvent where T : IRuntimeFactory
{
    private readonly EffectContext context;
    private readonly T affecter;
    private readonly LifetimeLogic lifetime;
    private Action<EffectContext, T> tickProc;
    private List<Action<EffectContext>> callbacks;
    public UnityEngine.Object Owner { get; set; }
    public bool IsFinished
    {
        get
        {
            bool isFinished = lifetime.ShouldDestroy(context);
            if(isFinished) {Debug.Log("Invoking callbacks"); callbacks?.ForEach(e => e.Invoke(context));}
            return isFinished;
        }
    }

    public RuntimeEvent(EffectContext context, T affecter, Action<EffectContext, T> tickProc, List<Action<EffectContext>> callbacks = null)
    {
        this.context = context;
        this.affecter = affecter;
        this.tickProc = tickProc;
        this.callbacks = callbacks;
        lifetime = affecter.Lifetime.Activate();
    }

    public static async Task<IRuntimeEvent> Create(EffectContext context, T affecter, Action<EffectContext, T> tickProc, List<Action<EffectContext>> callbacks = null)
    {
        RuntimeEvent<T> runtimeEvent = new (context, affecter, tickProc, callbacks);
        
        await affecter.Apply(context);
        
        runtimeEvent.Owner = context.Targets.Count > 0 ? context.Targets[0].Body : null;

        return runtimeEvent;
    }

    public void Tick() => tickProc?.Invoke(context, affecter);
}
