using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRunner : MonoBehaviour
{
    private readonly List<IEffectRuntime> runningEffects = new();

    public void Run(IAbilityEffect effect, EffectContext context)
    {
        runningEffects.Add(effect.CreateEffectRuntime(context));
    }

    private void Update()
    {
        TickEffects();
    }

    private void TickEffects()
    {
        for (int i = runningEffects.Count - 1; i >= 0; i--)
        {
            IEffectRuntime runtime = runningEffects[i];
            
            if(runtime != null)
            {
                runtime.Tick();

                if (runtime.IsFinished)
                    runningEffects.RemoveAt(i);
            }
            else
            {
                runningEffects.RemoveAt(i);
            }
            
        }
    }
}
