using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat : Effect
{
    [SerializeField] private Stats statType;
    [SerializeField] private bool isPositive;
    public Stat(Effect effect = null)
    {
         if(effect != null) (lifetimeLabel, lifetime, repeatLabel, runs, targetLabel, coordinator) = effect;
    }
}
