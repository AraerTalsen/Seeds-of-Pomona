using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlayerInventoryData")]
public class PlayerInventoryData : InventoryData
{
    [SerializeField] private int balance = 0;
    [SerializeField] private bool hasDied = false;
    [SerializeField] private List<InventoryEntry> powerups;
    [SerializeField] private List<bool> lockStates;

    public int Balance { get => balance; set => balance = value; }
    public bool HasDied { get => hasDied; set => hasDied = value; }
    public List<InventoryEntry> Powerups { get => powerups; set => powerups = value; }
    public List<bool> LockStates { get => lockStates; set => lockStates = value; }
}
