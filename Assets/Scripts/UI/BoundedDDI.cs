using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundedDDI : DragNDropInventory
{
    public BoundedDDI(Transform invContainer, bool hasListener = false)
    {
        DisplayManager = new(this, invContainer);
        Capacity = invContainer.childCount;

        InitializeInventory();
        Listener = hasListener ? new() : null;
    }

    public override void Delete(int slotIndex) 
    {
        Listener?.StartNewEvent();
        if(Listener != null)
        {
            Listener.TempItem = Read(slotIndex).Item;
        }
        
        _Inventory[slotIndex].Remove();
        Listener?.TouchSlot(slotIndex, Listener.TempItem, InventoryListener.SlotTouchMode.Cleared);
    }

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

    public override void ClearInventory()
    {
        for(int i = 0; i < _Inventory.Count; i++)
        {
            _Inventory[i].Remove();
        }
        DisplayManager.UpdateDisplayAll();
    }
}
