using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRunner : MonoBehaviour
{
    private readonly List<IEffectRuntime> runningEffects = new();

    public void Run(Tool powerup, PowerupContext context)
    {
        runningEffects.Add(powerup.CreateEffectRuntime(context));
    }

    private void Update()
    {
        TickEffects();
    }

    private void TickEffects()
    {
        for (int i = runningEffects.Count - 1; i >= 0; i--)
        {
            var runtime = runningEffects[i];

            runtime.Tick();

            if (runtime.IsFinished)
                runningEffects.RemoveAt(i);
        }
    }
}
