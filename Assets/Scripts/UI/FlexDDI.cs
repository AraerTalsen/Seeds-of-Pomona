using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexDDI : DragNDropInventory
{
    public FlexDDI(int capacity, Transform invContainer, FlexInvDisplayManager.ISlotPrefill prefill, bool hasListener = false)
    {
        DisplayManager = new FlexInvDisplayManager(this, invContainer, prefill);
        Capacity = capacity;

        InitializeInventory();
        Listener = hasListener ? new() : null;
    }

    public void Create(int qty, Item item)
    {
        _Inventory.Add(new InventoryEntry(qty, item));
        Listener?.TouchSlot(Count - 1, Read(Count - 1).Item, InventoryListener.SlotTouchMode.Created);

        ((FlexInvDisplayManager)DisplayManager).AddNewISlot();
    } 
    public void Create(InventoryEntry entry)
    {
        _Inventory.Add(entry);
        Listener?.TouchSlot(Count - 1, Read(Count - 1).Item, InventoryListener.SlotTouchMode.Created);

        ((FlexInvDisplayManager)DisplayManager).AddNewISlot();
    }
    

    public override void Delete(int slotIndex) 
    {
        Listener?.StartNewEvent();
        if(Listener != null)
        {
            Listener.TempItem = Read(slotIndex).Item;
        }
        
        _Inventory.RemoveAt(slotIndex);
        Listener?.TouchSlot(slotIndex, Listener.TempItem, InventoryListener.SlotTouchMode.Deleted);
    }

    protected override void UpdateQty(int qty, Item item, out int remainder)
    {
        base.UpdateQty(qty, item, out remainder);
        if(remainder > 0)
        {
            TryAppend(remainder, item);
        }
    }

    protected override void InitializeInventory()
    {
        CreateNewEntry();
    }

    private void TryAppend(int qty, Item item)
    {
        if(!IsAtCapacity())
        {
            int stacks = qty / item.maxStackSize;
            int remainder = qty % item.maxStackSize;

            for(int i = 0; i < stacks; i++)
            {
                Create(item.maxStackSize, item);
            }

            Create(remainder, item);
        }
    }

    public bool IsAtCapacity()
    {
        if(Capacity > -1 && Count == Capacity)
        {
            Debug.Log($"Failed to add a new slot. Inventory is at capacity");
            return true;
        }

        return false;
    }

    public override void ClearInventory()
    {
        _Inventory.Clear();
        CreateNewEntry();
        DisplayManager.UpdateDisplayAll();
    }
}
