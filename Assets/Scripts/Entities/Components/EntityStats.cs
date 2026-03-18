using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour, IHealth
{
    [SerializeField] private StatBlock stats = new();
    [SerializeField] private EntityDeathManager deathManager;
    public StatBlock StatBlock { get => stats; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    private int currentHealth;

    private void Start()
    {
        MaxHealth = StatBlock.GetModdedStat(Stats.Health);
        CurrentHealth = MaxHealth;
    }
}
