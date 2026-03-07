using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilityEffect
{
    public abstract IEffectRuntime CreateEffectRuntime(EffectContext context); 
}
