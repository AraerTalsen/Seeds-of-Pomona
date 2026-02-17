using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundedDDI : DragNDropInventory
{
    public BoundedDDI(Transform invContainer)
    {
        DisplayManager = new(this, invContainer);
        Capacity = invContainer.childCount;

        InitializeInventory();
    }

    public override void Delete(int slotIndex) => _Inventory[slotIndex].Remove();

    public override void LoadFromStorage(List<InventoryEntry> storedData)
    {
        base.LoadFromStorage(storedData);
        DisplayManager.InitializeInventoryDisplay();
    }

    protected override void InitializeInventory()
    {
        for(int i = 0; i < Capacity; i++)
        {
            CreateNewEntry();
        }
    }
}
