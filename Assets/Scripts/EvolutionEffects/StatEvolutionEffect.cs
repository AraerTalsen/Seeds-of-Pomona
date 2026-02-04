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
        StatPayload data = (StatPayload)payload;
        for(int i = 0; i < data.stats.Count; i++)
        {
            context.stats.Modify(data.stats[i], data.mods[i]);
        }
    }
}
