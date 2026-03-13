using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Basic Ability State")]
public class BasicAbilityState : AbilityState
{
    [SerializeField] private float coolDown;
    public override float RecoveryTime => coolDown;
    public override bool IsValid => !IsCoolingDown;
    public override IEffectRuntime CreateEffectRuntime(EffectContext context)
    {
        ResetContextState();
        return effect.CreateRuntime(context);
    }
}
