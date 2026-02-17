using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexDDI : DragNDropInventory
{
    public FlexDDI(int capacity, Transform invContainer, FlexInvDisplayManager.ISlotPrefill prefill)
    {
        DisplayManager = new FlexInvDisplayManager(this, invContainer, prefill);
        Capacity = capacity;

        InitializeInventory();
    }

    public void Create(int qty, Item item)
    {
        _Inventory.Add(new InventoryEntry(qty, item));

        ((FlexInvDisplayManager)DisplayManager).AddNewISlot();
    } 
    public void Create(InventoryEntry entry)
    {
        _Inventory.Add(entry);

        ((FlexInvDisplayManager)DisplayManager).AddNewISlot();
    }
    

    public override void Delete(int slotIndex) => _Inventory.RemoveAt(slotIndex);

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
}
