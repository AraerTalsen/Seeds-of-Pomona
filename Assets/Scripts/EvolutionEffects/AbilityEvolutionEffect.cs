using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AbilityEvolutionEffect")]
public class AbilityEvolutionEffect : EvolutionEffect
{
    public override Type PayloadType => typeof(AbilityPayload);
    
    protected override void HandleEffect(NPCEffectContext context, Payload payload, bool isApplied)
    {
        AbilityPayload data = (AbilityPayload)payload;
        IBehaviorContext hostContext = GetHostContextFromPayload(context.stateMachine, data.ability.HostContext);
        if(isApplied)
            hostContext.AddState(Instantiate(data.ability), data.probability);
        else
            hostContext.RemoveState(data.ability, data.probability);
    }

    private IBehaviorContext GetHostContextFromPayload(BehaviorContext startNode, Type targetContextType) => startNode.ContextRegistry[targetContextType];
}
