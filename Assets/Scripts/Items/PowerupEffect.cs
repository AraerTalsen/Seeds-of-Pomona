using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerupEffect : ScriptableObject
{
    public abstract IEffectRuntime CreateRuntime(PowerupContext context);
    protected abstract void Apply(PowerupContext context);
}
