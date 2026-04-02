using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class StatBlock
{
    [SerializeField] private int[] baseStats = new int[Enum.GetValues(typeof(Stats)).Length];
    private int[] modifiers = new int[Enum.GetValues(typeof(Stats)).Length];
    private Dictionary<Stats, List<IStatReact>> subscribers = new()
    {
        {Stats.Health, new()},
        {Stats.Speed, new()},
        {Stats.Strength, new()}
    }; 

    public int this[Stats stat] => baseStats[(int)stat];

    public int GetModdedStat(Stats stat) => baseStats[(int)stat] + modifiers[(int)stat];

    public void AddTo(Stats stat, int val) => UpdateStatMod(stat, val);
    public void SubtractFrom(Stats stat, int val) => UpdateStatMod(stat, -val);
    private void UpdateStatMod(Stats stat, int val)
    {
        modifiers[(int)stat] += val;
        BroadcastTo(stat);
    }

    public float GetStatLvlConvertVal(Stats stat) => StatDefinitions.Get(stat).Apply(GetModdedStat(stat));
    public void SubscribeToStat(Stats stat, IStatReact subscriber)
    {
        if(!subscribers[stat].Contains(subscriber))
        {
            subscribers[stat].Add(subscriber);
        }
    }

    public void UnsubscribeFromStat(Stats stat, IStatReact subscriber)
    {
        if(subscribers[stat].Contains(subscriber))
        {
            subscribers[stat].Remove(subscriber);
        }
    }

    private void BroadcastTo(Stats stat)
    {
        foreach(IStatReact subscriber in subscribers[stat])
        {
            subscriber.ReactToStatChange(stat);
        }
    }
    
    public void ValidateSize()
    {
        int statCount = Enum.GetValues(typeof(Stats)).Length;

        if (baseStats == null || baseStats.Length != statCount)
        {
            Array.Resize(ref baseStats, statCount);
        }
    }
}
