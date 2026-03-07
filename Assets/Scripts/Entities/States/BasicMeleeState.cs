using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeState : BehaviorState
{
    public override float RecoveryTime => 7;
    public override bool IsValid => !IsCoolingDown;
    protected override string LoadEffect => "ScriptableObjects/PowerupFunctions/CreateStatContact";
    public override IEffectRuntime CreateEffectRuntime(EffectContext effectContext) 
    {
        ((CombatState)Context).UpdateMoveSet(this, true);
        ResetContextState();
        return powerupEffect.CreateRuntime(effectContext);
    }
}
