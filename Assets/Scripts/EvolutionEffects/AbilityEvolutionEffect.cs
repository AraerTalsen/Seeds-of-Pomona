using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AbilityEvolutionEffect")]
public class AbilityEvolutionEffect : EvolutionEffect
{
    public override Type PayloadType => typeof(AbilityPayload);

    public override void Apply(EvolutionContext context, Payload payload)
    {
        HandleEffect(context, payload, true);
    }

    public override void Revert(EvolutionContext context, Payload payload)
    {
        HandleEffect(context, payload, false);
    }

    protected override void HandleEffect(EvolutionContext context, Payload payload, bool isApplied)
    {
        AbilityPayload data = (AbilityPayload)payload;
        if(isApplied)
            context.stateMachine.AddState(data.ability, data.probabilty);
        else
            context.stateMachine.RemoveState(data.ability, data.probabilty);
    }
}
