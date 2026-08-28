using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/InventoryData")]
public class InventoryData : PersistentDataBase
{
    [System.Serializable]
    public struct KeyIndexPair
    {
        [SerializeField] private int _index;
        [SerializeField] private string _key;

        public int Index { get => _index; set => _index = value; }
        public string Key { get => _key; set => _key = value; }
    }

    [SerializeField] private List<InventoryEntry> inventory = new();

    [SerializeField]
    private bool isPersisting = false;
    [SerializeField] private List<KeyIndexPair> keyIndexPairs = new();
    public bool IsPersisting { get => isPersisting; set => isPersisting = value; }

    public void SaveInventory(List<InventoryEntry> entries)
    {
        keyIndexPairs.Clear();
        for(int i = 0; i < entries.Count; i++)
        {
            if(!entries[i].IsEmpty)
            {
                string cloneKey = entries[i].Item.CloneKey;
                if(cloneKey.CompareTo("") != 0)
                {
                    keyIndexPairs.Add(new KeyIndexPair { Index = i, Key = cloneKey });
                    entries[i].Remove();
                }
            }
        }
        inventory = entries;
    }

    public List<InventoryEntry> LoadInventory()
    {
        foreach(var pair in keyIndexPairs)
        {
            inventory[pair.Index].Set(1, (Item)UniqueItemManager.Load(pair.Key));
        }
        return inventory;
    }
    
    public void ClearInventory()
    {
        for(int i = 0; i < inventory.Count; i++)
        {
            if(!inventory[i].IsEmpty)
            {
                string cloneKey = inventory[i].Item.CloneKey;
                if(cloneKey.CompareTo("") != 0)
                    UniqueItemManager.Delete(cloneKey);
                inventory[i].Remove();
            }
        }
    }
}
