using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundedInventory : ItemInventory
{
    public BoundedInventory(int maxCapacity)
    {
        for(int i = 0; i < maxCapacity; i++)
        {
            _Inventory.Add(InventoryEntry.Empty());
        }
    }

    public override void Delete(int slotIndex) => _Inventory[slotIndex].Remove();
}
