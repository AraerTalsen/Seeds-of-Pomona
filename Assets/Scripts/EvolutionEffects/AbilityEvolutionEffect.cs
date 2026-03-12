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
        BehaviorContext hostContext = GetHostContextFromPayload(context.stateMachine, data.ability.HostContext);
        if(isApplied)
            hostContext.AddState(data.ability, data.probability);
        else
            hostContext.RemoveState(data.ability, data.probability);
    }

    private BehaviorContext GetHostContextFromPayload(BehaviorContext startNode, Type targetContextType)
    {
        Stack<BehaviorContext> stack = new();
        stack.Push(startNode);

        while (stack.Count > 0)
        {
            BehaviorContext current = stack.Pop();

            if (targetContextType.Equals(current.GetType()))
                return current;

            foreach ((IBehaviorState state, _) in current.PossibleStates)
            {
                if (state is BehaviorContext childContext)
                    stack.Push(childContext);
            }
        }

        return null;
    }
}
