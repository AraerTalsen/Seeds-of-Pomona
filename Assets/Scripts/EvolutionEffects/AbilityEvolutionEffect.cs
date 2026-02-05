using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AbilityEvolutionEffect")]
public class AbilityEvolutionEffect : EvolutionEffect
{
    public override Type PayloadType => typeof(AbilityPayload);
    
    protected override void HandleEffect(EvolutionContext context, Payload payload, bool isApplied)
    {
        AbilityPayload data = (AbilityPayload)payload;
        if(isApplied)
            context.stateMachine.AddState(data.ability, data.probability);
        else
            context.stateMachine.RemoveState(data.ability, data.probability);
    }
}
