using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBlock : MonoBehaviour
{
    [SerializeField ] private Dictionary<Stats, int> stats = new (Enum.GetNames(typeof(Stats)).Length); 
    private Dictionary<Stats, int> modifiers = new (Enum.GetNames(typeof(Stats)).Length);  

    public void Modify(Stats stat, int mod)
    {
        modifiers[stat]+= mod;
    }

    public int GetStat(Stats stat)
    {
        return stats[stat] + modifiers[stat];
    }
}
