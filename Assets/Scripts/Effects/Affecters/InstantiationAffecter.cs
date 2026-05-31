using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiationAffecter : Affecter<InstantiationAffecter>
{
    public enum ParentSelect { none, host, target }
    [SerializeField] private GameObject node;
    [SerializeField] private ParentSelect parentSelection;

    public InstantiationAffecter(EffectParameters parameters)
    {
        (
            lockMovement, stackableUntil,
            lifetimeLabel, lifetime,
            repeatLabel, runs,
            targetLabel, coordinator
        ) = parameters;    
    }

    public InstantiationAffecter(Affecter affecter = null)
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
