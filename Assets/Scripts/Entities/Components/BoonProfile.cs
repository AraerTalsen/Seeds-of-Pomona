using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoonProfile
{
    private Dictionary<Stats, int> modifiers = new();
    public Dictionary<Stats, int> Modifiers { get => modifiers; private set => modifiers = value; }
    private BoonDisplay boonDisplay;
    private StatBlock statBlock;

    public BoonProfile(BoonDisplay.BoonDisplayProps props, StatBlock stats)
    {
        InitializeProfile();
        boonDisplay = new(props);
        statBlock = stats;
    }

    private void InitializeProfile()
    {
        Modifiers.Clear();
        foreach(Stats s in Enum.GetValues(typeof(Stats)))
        {
            Modifiers.Add(s, 0);
        }
    }

    public void BoostStat(Stats stat, int amount)
    {
        modifiers[stat] += amount;
        statBlock[stat] += amount;
        boonDisplay.UpdateStatDisplay(stat, modifiers[stat]);
    }

    public void LoadModifiers(Dictionary<Stats, int> modifiers)
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if(sceneName.CompareTo("Wilderness") == 0)
        {
            Modifiers = modifiers;
            foreach(KeyValuePair<Stats, int> pair in Modifiers) 
            {
                Debug.Log($"{pair.Key} -> {pair.Value}");
                boonDisplay.UpdateStatDisplay(pair.Key, pair.Value);
            }
        }
        else
        {
            Modifiers = modifiers;
            InitializeProfile();
        }
    }
}
