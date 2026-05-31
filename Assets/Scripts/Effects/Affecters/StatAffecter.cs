using System;
using System.Collections;
using System.Collections.Generic;
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
        RegisterRulebook(StatEffectRulebook.Instance);
        (
            lockMovement, stackableUntil,
            lifetimeLabel, lifetime,
            repeatLabel, runs,
            targetLabel, coordinator
        ) = parameters;    
    }

    public StatAffecter(Affecter affecter = null)
    {
        RegisterRulebook(StatEffectRulebook.Instance);
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
}
