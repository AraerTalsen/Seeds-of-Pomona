using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlayerInventoryData")]
public class PlayerInventoryData : InventoryData
{
    [SerializeField] private int balance = 0;
    [SerializeField] private bool hasDied = false;
    [SerializeField] private List<InventoryEntry> powerups;
    [SerializeField] private List<InventoryEntry> hotbar;
    [SerializeField] private List<bool> lockStates;
    [SerializeField] private Dictionary<Stats, int> boons;

    public int Balance { get => balance; set => balance = value; }
    public bool HasDied { get => hasDied; set => hasDied = value; }
    public List<InventoryEntry> Powerups { get => powerups; set => powerups = value; }
    public List<InventoryEntry> Hotbar { get => hotbar; set => hotbar = value; }
    public List<bool> LockStates { get => lockStates; set => lockStates = value; }
    public Dictionary<Stats, int> Boons { get => boons; set => boons = value; }

    public void ClearPowerups()
    {
        for(int i = 0; i < Powerups.Count; i++)
        {
            Item item = Powerups[i].Item;
            Powerups[i].Remove();
            if(item is PUp) UniqueItemManager.Delete(item);
        }
    }

    public void ClearHotbar()
    {
        for(int i = 0; i < Hotbar.Count; i++)
        {
            Item item = Hotbar[i].Item;
            Hotbar[i].Remove();
            if(item is PUp) UniqueItemManager.Delete(item);
        }
    }
}
