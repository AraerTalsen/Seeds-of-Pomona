using System;
using System.Collections;
using System.Collections.Generic;
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
            runtime = CreateRuntimeEvent(context);
            EventRunner.Run(runtime);
        }

        public void Apply(EffectContext context) {}

        public IRuntimeEvent CreateRuntimeEvent(EffectContext context)
        {
            return new RuntimeEvent<Wrapper>(context, this, null, _callback);
        }
    }

    
}
