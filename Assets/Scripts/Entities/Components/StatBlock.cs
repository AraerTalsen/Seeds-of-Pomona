using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class StatBlock
{
    //[SerializeField] private GameObject statDebugDisplay;
    //[SerializeField] private List<StatValPair> statValPairs;
    [SerializeField] private int[] baseStats = new int[Enum.GetValues(typeof(Stats)).Length];
    private int[] modifiers = new int[Enum.GetValues(typeof(Stats)).Length]; 
    //private bool debugMode = false;
    //private TextMeshProUGUI statDisplay;

    public int this[Stats stat]
    {
        get => baseStats[(int)stat] + modifiers[(int)stat];
        set => modifiers[(int)stat] = value;
    }

    public void ValidateSize()
    {
        int statCount = Enum.GetValues(typeof(Stats)).Length;

        if (baseStats == null || baseStats.Length != statCount)
        {
            Array.Resize(ref baseStats, statCount);
        }
    }

    /*private void Update()
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
    }*/
}
