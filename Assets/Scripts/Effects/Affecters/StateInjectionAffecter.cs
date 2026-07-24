using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateInjectionAffecter : Affecter<StateInjectionAffecter>
{
    public StateInjectionAffecter(EffectParameters parameters)
    {
        (
            lockMovement, stackableUntil,
            lifetimeLabel, lifetime,
            repeatLabel, runs,
            targetLabel, coordinator
        ) = parameters;    
    }

    public StateInjectionAffecter(Affecter affecter = null)
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
