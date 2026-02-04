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
        AbilityPayload data = (AbilityPayload)payload;
        context.stateMachine.AddState(data.ability, data.probabilty);
    }
}
