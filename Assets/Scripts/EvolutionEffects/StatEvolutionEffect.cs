using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/StatEvolutionEffect")]
public class StatEvolutionEffect : EvolutionEffect
{
    public override Type PayloadType => typeof(StatPayload);

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
        StatPayload data = (StatPayload)payload;
        int sign = isApplied ? 1 : -1;

        for(int i = 0; i < data.statValPair.Count; i++)
        {
            StatValPair pair = data.statValPair[i];
            context.stats.Modify(pair.stat, sign * pair.val);
        }
    }
}
