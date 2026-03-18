using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/StatEvolutionEffect")]
public class StatEvolutionEffect : EvolutionEffect
{
    public override Type PayloadType => typeof(StatPayload);
    protected override void HandleEffect(NPCEffectContext context, Payload payload, bool isApplied)
    {
        StatPayload data = (StatPayload)payload;
        int sign = isApplied ? 1 : -1;

        for(int i = 0; i < data.statValPairs.Count; i++)
        {
            StatValPair pair = data.statValPairs[i];
            if(sign > 0)
            {
                context.stats.AddTo(pair.stat, pair.val);
            }
            else
            {
                context.stats.SubtractFrom(pair.stat, pair.val);
            }
        }
    }
}
