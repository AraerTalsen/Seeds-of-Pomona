using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatBlock : MonoBehaviour
{
    [SerializeField] private GameObject statDebugDisplay;
    [SerializeField] private List<StatValPair> statValPairs;
    private Dictionary<Stats, int> baseStats = new(); 
    private Dictionary<Stats, int> modifiers = new();
    private bool debugMode = false;
    private TextMeshProUGUI statDisplay;

    private void Awake()
    {
        InitializeValues();
    }

    private void Update()
    {
        DebugModeCheck();

        if(debugMode)
        {
            StatDebugger();
        }
        
    }

    private void DebugModeCheck()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            debugMode = !debugMode;
            statDebugDisplay.SetActive(debugMode);
        }
    }

    private void StatDebugger()
    {
        KeyValuePair<Stats, int>[] pairs = baseStats.ToArray();
        string display = "";
        for(int i = 0; i < pairs.Length; i++)
        {
            display += $"{pairs[i].Key}: {pairs[i].Value}({modifiers[pairs[i].Key]}) ";
        }
        statDisplay.text = display;
    }

    private void InitializeValues()
    {
        for(int i = 0; i < statValPairs.Count; i++)
        {
            baseStats.Add(statValPairs[i].stat, statValPairs[i].val);
            modifiers.Add(statValPairs[i].stat, 0);
        }
        statDisplay = statDebugDisplay.GetComponent<TextMeshProUGUI>();
    }

    public void Modify(Stats stat, int mod)
    {
        modifiers[stat]+= mod;
    }

    public int GetStat(Stats stat)
    {
        return baseStats[stat] + modifiers[stat];
    }
}
