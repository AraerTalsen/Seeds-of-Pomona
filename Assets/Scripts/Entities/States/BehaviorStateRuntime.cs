using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BehaviorStateRuntime : BehaviorState
{
    private IRuntimeEvent runtimeEvent;
    protected class StateRuntime : IRuntimeEvent
    {
        private EffectContext _context;
        private Action<EffectContext> _tickProc;
        private Func<EffectContext, bool> _endCondition;
        private Action<EffectContext> _callback;
        
        public UnityEngine.Object Owner { get; set; }
        
        public bool IsFinished
        {
            get
            {
                bool isDone = _endCondition.Invoke(_context);
                if(isDone) _callback.Invoke(_context);
                return isDone;
            }
        }

        public StateRuntime
        (
            EffectContext context, 
            Action<EffectContext> tickProcess, 
            Func<EffectContext, bool> endCondition, 
            Action<EffectContext> callback
        )
        {
            Owner = context.Owner.Body;
            _context = context;
            _tickProc = tickProcess;
            _endCondition = endCondition;
            _callback = callback;
        }

        public void Tick() => _tickProc.Invoke(_context);
    }

    public override Task<IRuntimeEvent> LaunchEffect(EffectContext effectContext)
    {
        if(runtimeEvent == null)
        {
            runtimeEvent = new StateRuntime(effectContext, TickProcess, EndCondition, Callback);
            return Task.FromResult(runtimeEvent);
        }
        
        return Task.FromResult<IRuntimeEvent>(null);
    }

    protected override void ResetContextState()
    {
        runtimeEvent = null;
        base.ResetContextState();
    }

    public abstract void TickProcess(EffectContext context);
    public abstract bool EndCondition(EffectContext context);
}
