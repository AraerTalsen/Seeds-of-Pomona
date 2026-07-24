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

        public LifetimeLogic Lifetime => new ConditionalLifetime(new()
        { 
            Resources.Load<LifetimeRule>("ScriptableObjects/LifetimeRules/UnderSpeed"),
            Resources.Load<LifetimeRule>("ScriptableObjects/LifetimeRules/AnyContact")
        });

        public Wrapper(EffectContext context, Action<EffectContext> callback)
        {
            _callback = callback;
            runtime = CreateRuntimeEventSync(context);
            EventRunner.Run(runtime);
        }

        public Task Apply(EffectContext context) => Task.CompletedTask;

        public Task<IRuntimeEvent> CreateRuntimeEvent(EffectContext context, Action<EffectContext> callback = null)
        {
            return RuntimeEvent<Wrapper>.Create(context, this, null, new List<Action<EffectContext>> {_callback});
        }

        private IRuntimeEvent CreateRuntimeEventSync(EffectContext context)
        {
            return RuntimeEvent<Wrapper>.Create(context, this, null, new List<Action<EffectContext>> { _callback }).GetAwaiter().GetResult();
        }
        
    }    
}
