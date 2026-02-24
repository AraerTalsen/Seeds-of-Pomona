using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryBase
{
    protected List<InventoryEntry> _Inventory { get; private set; } = new();
    public List<InventoryEntry> Entries => _Inventory;
    public int Count => _Inventory.Count; 

    public virtual void CreateNewEntry() => _Inventory.Add(InventoryEntry.Empty()); 

    public InventoryEntry Read(int slotIndex) => _Inventory[slotIndex];
    public InventoryEntry Find(Item item)
    {
        for(int i = 0; i < _Inventory.Count; i++)
        {
            if(_Inventory[i].Item.id == item.id)
            {
                return _Inventory[i];
            }
        }
        return null;
    }

    public InventoryEntry Find(int id)
    {
        for(int i = 0; i < _Inventory.Count; i++)
        {
            Item item = _Inventory[i].Item;

            if(item != null && item.id == id)
            {
                return _Inventory[i];
            }
        }
        return null;
    }

    public List<int> FindAll(Item item)
    {
        List<int> locations = new();

        for(int i = 0; i < _Inventory.Count; i++)
        {
            Item thisItem = _Inventory[i].Item;

            bool nullCheck = item == null && thisItem == null;
            bool itemCheck = item != null && thisItem != null && thisItem.id == item.id;
            if(nullCheck || itemCheck)
            {
                locations.Add(i);
            }
        }

        return locations;
    }
    public List<int> FindAll(int id)
    {
        List<int> locations = new();

        for(int i = 0; i < _Inventory.Count; i++)
        {
            if(_Inventory[i].Item != null && _Inventory[i].Item.id == id)
            {
                locations.Add(i);
            }
        }

        return locations;
    }

    public int TryAddQuantity (int qty, int slotIndex) => Read(slotIndex).TryAddQuantity(qty);
    public int SetItem(int qty, Item item, int slotIndex) => _Inventory[slotIndex].Set(qty, item);
    public virtual void LoadFromStorage(List<InventoryEntry> storedData) => _Inventory = storedData;
}
