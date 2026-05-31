using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour, IHealth
{
    [SerializeField] private float iFrameDurration;
    [SerializeField] private StatBlock stats = new();
    public StatBlock StatBlock { get => stats; }
    public int MaxHealth { get; set; }
    public int CurrentHealth 
    { 
        get => currentHealth; 
        set
        {
            if(!isRecovering)
            {
                if(value < currentHealth)
                {
                    isRecovering = true;
                    iFrameStartTime = Time.time;
                }
                
                currentHealth = value; 
            }
        }
    }
    private int currentHealth;
    private bool isRecovering = false;
    private float iFrameStartTime;

    private void Start()
    {
        MaxHealth = StatBlock.GetModdedStat(Stats.Health);
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        if(isRecovering) MaintainIFrames();
    }

    private void MaintainIFrames() => isRecovering = Time.time < iFrameStartTime + iFrameDurration;

    public void AddTo(Stats stat, int val) => StatBlock.AddTo(stat, val);
    public void SubtractFrom(Stats stat, int val) => StatBlock.SubtractFrom(stat, -val);

    public void SubscribeToStatChange(Stats stat, IStatReact subscriber) => StatBlock.SubscribeToStat(stat, subscriber);
    public void UnsubscribeFromStatChange(Stats stat, IStatReact subscriber) => StatBlock.UnsubscribeFromStat(stat, subscriber);
}
