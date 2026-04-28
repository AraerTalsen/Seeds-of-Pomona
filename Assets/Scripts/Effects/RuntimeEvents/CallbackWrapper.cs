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
        //private Func<T> _getter;
        private Action<EffectContext> _callback;
        //private T previousValue;
        private IRuntimeEvent runtime;

        public LifetimeLogic Lifetime => new ConditionalLifetime(Resources.Load<LifetimeRule>("ScriptableObjects/LifetimeRules/UnderSpeed"));

        public Wrapper(EffectContext context, Action<EffectContext> callback)
        {
            //_getter = getter;
            _callback = callback;
            //previousValue = getter();
            runtime = CreateRuntimeEvent(context);
            EventRunner.Run(runtime);
        }

        public void Listen(EffectContext context, Wrapper wrapper)
        {
            Debug.Log($"Velocity: {context.target.GetComponent<Rigidbody2D>().velocity.magnitude}");
            Debug.Log("Printing rule: " + ((ConditionalLifetime)Lifetime).Rule);
            /*T current = _getter();
            if (!EqualityComparer<T>.Default.Equals(current, previousValue))
            {
                Debug.Log("Terminate listener");
                EventRunner.Terminate(runtime);
            }*/
        }

        public void Apply(EffectContext context) {}

        public IRuntimeEvent CreateRuntimeEvent(EffectContext context)
        {
            return new RuntimeEvent<Wrapper>(context, this, Listen, _callback);
        }
    }

    
}
