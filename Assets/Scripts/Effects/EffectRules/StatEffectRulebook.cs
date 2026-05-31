using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StatEffectRulebook : IEffectRulebook<StatAffecter>
{
    public static StatEffectRulebook Instance { get; } = new();
    
    Dictionary<int, Action<EffectContext, StatAffecter>> IEffectRulebook<StatAffecter>.EffectDirectory => new()
    {
        {1, ModifyStat},
    };

    private void ModifyStat(EffectContext context, StatAffecter payload)
    {
        List<GameObject> hits = context.Targets.Select(e => e.Body).ToList();
        hits.RemoveAll( g => g.layer != 3 && g.GetComponent<EntityStats>() == null);

        Stats statType = payload.StatType;
        bool isPositive = payload.IsPositive;
        int magnitude = payload.Magnitude;

        for(int i = 0; i < hits.Count; i++)
        { 
            EntityStats stats = hits[i].GetComponent<EntityStats>();

            if(payload.IsResourcePoints && statType == Stats.Health)
            {
                AffectResourcePoints(isPositive, stats, magnitude);
            }
            else
            {
                AffectRawStat(isPositive, stats, statType, magnitude);
            }
            

            if(payload.TargetMode == Affecter.TargetLabel.single) break;
        }
    }

    private void AffectRawStat(bool isPositive, EntityStats stats, Stats statType, int magnitude)
    {
        if(isPositive)
        {
            stats.AddTo(statType, magnitude);
        }
        else
        {
            stats.SubtractFrom(statType, magnitude);
        }
    }

    private void AffectResourcePoints(bool isPositive, EntityStats stats, int magnitude)
    {
        stats.CurrentHealth += isPositive ? magnitude : -magnitude;
    }

    bool[] IEffectRulebook<StatAffecter>.EffectConfig(StatAffecter payload)
    {
        bool[] arr =
        {
            true,
        };
        return arr;
    }
}
