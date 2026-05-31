using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class CallbackWrapper
{
    public static Action<EffectContext> WrapAction(Action<EffectContext> callback = null)
    {
        return callback == null ? callback : context => new Wrapper(context, callback);
    }

    private class Wrapper : IRuntimeFactory
    {
        private Action<EffectContext> _callback;
        private IRuntimeEvent runtime;

        public LifetimeLogic Lifetime => new ConditionalLifetime(Resources.Load<LifetimeRule>("ScriptableObjects/LifetimeRules/UnderSpeed"));

        public Wrapper(EffectContext context, Action<EffectContext> callback)
        {
            _callback = callback;
            runtime = (IRuntimeEvent)CreateRuntimeEvent(context);
            EventRunner.Run(runtime);
        }

        public Task Apply(EffectContext context) => Task.CompletedTask;

        public Task<IRuntimeEvent> CreateRuntimeEvent(EffectContext context, Action<EffectContext> callback = null)
        {
            return RuntimeEvent<Wrapper>.Create(context, this, null, new List<Action<EffectContext>> {_callback});
        }
    }

    
}
