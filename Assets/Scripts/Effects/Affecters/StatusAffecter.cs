using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusAffecter : Affecter<StatusAffecter>
{
    public enum StatusLabel { stun }

    [SerializeField] private StatusLabel statusLabel;
    
    public StatusAffecter(EffectParameters parameters)
    {
        (
            lockMovement, stackableUntil,
            lifetimeLabel, lifetime,
            repeatLabel, runs,
            targetLabel, coordinator
        ) = parameters;    
    }

    public StatusAffecter(Affecter affecter = null)
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
}
