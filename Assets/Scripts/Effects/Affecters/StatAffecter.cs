using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class StatAffecter : Affecter<StatAffecter>
{
    [SerializeField] private Stats statType;
    [SerializeField] private bool isResourcePoints;
    [SerializeField] private bool isPositive;
    [SerializeField] private int magnitude;

    public Stats StatType => statType;
    public bool IsResourcePoints => isResourcePoints;
    public bool IsPositive => isPositive;
    public int Magnitude => magnitude;
    
    public StatAffecter(EffectParameters parameters)
    {
       (
            lockMovement, stackableUntil,
            lifetimeLabel, lifetime,
            repeatLabel, runs,
            targetLabel, coordinator
        ) = parameters;    
    }

    public StatAffecter(Affecter affecter = null)
    {
        if(affecter != null) 
        {
            (
                lockMovement, stackableUntil,
                lifetimeLabel, lifetime,
                repeatLabel, runs,
                targetLabel, coordinator
            ) = affecter;
        }   
    }

    public override Task<IRuntimeEvent> CreateRuntimeEvent(EffectContext context, Action<EffectContext> callback = null)
    {
        RegisterRulebook(StatEffectRulebook.Instance);
        return base.CreateRuntimeEvent(context, callback);
    }
}
