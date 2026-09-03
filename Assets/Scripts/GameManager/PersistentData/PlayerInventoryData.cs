using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlayerInventoryData")]
public class PlayerInventoryData : InventoryData
{
    [SerializeField] private int balance = 0;
    [SerializeField] private bool hasDied = false;
    [SerializeField] private List<InventoryEntry> powerups = new();
    [SerializeField] private List<InventoryEntry> hotbar = new();
    [SerializeField] private List<KeyIndexPair> hotbarKeys = new();
    [SerializeField] private List<KeyIndexPair> powerupKeys = new();
    [SerializeField] private List<bool> lockStates = new();
    [SerializeField] private Dictionary<Stats, int> boons;

    public int Balance { get => balance; set => balance = value; }
    public bool HasDied { get => hasDied; set => hasDied = value; }
    public List<bool> LockStates { get => lockStates; set => lockStates = value; }
    public Dictionary<Stats, int> Boons { get => boons; set => boons = value; }

    public void SavePowerups(List<InventoryEntry> entries)
    {
        powerupKeys.Clear();
        for(int i = 0; i < entries.Count; i++)
        {
            if(!entries[i].IsEmpty)
            {
                string cloneKey = entries[i].Item.CloneKey;
                if(cloneKey.CompareTo("") != 0)
                {
                    powerupKeys.Add(new KeyIndexPair { Index = i, Key = cloneKey });
                    entries[i].Remove();
                }
            }
        }
        powerups = entries;
    }
    public void SaveHotbar(List<InventoryEntry> entries)
    {
        hotbarKeys.Clear();
        for(int i = 0; i < entries.Count; i++)
        {
            if(!entries[i].IsEmpty)
            {
                string cloneKey = entries[i].Item.CloneKey;
                if(cloneKey.CompareTo("") != 0)
                {
                    hotbarKeys.Add(new KeyIndexPair { Index = i, Key = cloneKey });
                    entries[i].Remove();
                }
            }
        }
        hotbar = entries;
    }

    public List<InventoryEntry> LoadPowerups()
    {
        foreach(var pair in powerupKeys)
        {
            powerups[pair.Index].Set(1, (Item)UniqueItemManager.Load(pair.Key));
        }
        return powerups;
    }

    public List<InventoryEntry> LoadHotbar()
    {
        foreach(var pair in hotbarKeys)
        {
            hotbar[pair.Index].Set(1, (Item)UniqueItemManager.Load(pair.Key));
        }
        return hotbar;
    }
    
    public void ClearPowerups()
    {
        for(int i = 0; i < powerups.Count; i++)
        {
            if(!powerups[i].IsEmpty)
            {
                string cloneKey = powerups[i].Item.CloneKey;
                if(cloneKey.CompareTo("") != 0) 
                    UniqueItemManager.Delete(cloneKey);
                powerups[i].Remove();
            }
        }
    }

    public void ClearHotbar()
    {
        for(int i = 0; i < hotbar.Count; i++)
        {
            if(!hotbar[i].IsEmpty)
            {
                string cloneKey = hotbar[i].Item.CloneKey;
                if(cloneKey.CompareTo("") != 0)
                    UniqueItemManager.Delete(cloneKey);
                hotbar[i].Remove();
            }
        }
    }
}
