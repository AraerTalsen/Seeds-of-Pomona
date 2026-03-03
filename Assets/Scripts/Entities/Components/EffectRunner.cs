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

            //The only state that should have been returning a null value is the EnemyBehaviorContext, but its conditions to return null shouldn't
            //have succeeded. Find out why we were receiving null IEffectRuntimes
            if(runtime != null)
            {
                print(runtime.EffectName);
                runtime.Tick();

                if (runtime.IsFinished)
                    runningEffects.RemoveAt(i);
            }
            else
            {
                print("Discarded null runtime");
                runningEffects.RemoveAt(i);
            }
            
        }
    }
}
