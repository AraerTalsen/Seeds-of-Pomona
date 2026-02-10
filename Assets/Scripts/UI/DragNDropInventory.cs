using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNDropInventory : BoundedInventory
{
    private InventoryDisplayManager InventoryDisplayManager { get; set; }
    public DragNDropInventory(int maxCapacity, Transform invContainer) : 
    base(maxCapacity)
    {
        InventoryDisplayManager = new (this, invContainer);
    }

    public void SetSlot(InventoryEntry entry, int slotIndex)
    {
        SetEntry(entry, slotIndex);
        InventoryDisplayManager.UpdateItemDisplay(slotIndex);
    }

    public InventoryEntry PullSlot(int slotIndex)
    {
        InventoryEntry output = Read(slotIndex).Clone();

        Delete(slotIndex);
        InventoryDisplayManager.UpdateItemDisplay(slotIndex);

        return output;
    }

    public (int qty, Item item) PullItems(int id, int requestedQty, out int unfulfilled)
    {
        int totalQty = Sum(id);
        int outputQty = Mathf.Min(totalQty, requestedQty);
        InventoryEntry entry = Find(id);

        if(entry == null)
        {
            throw new ArgumentException($"Requested item of id: {id} does not exist in the inventory");
        }

        Item item = entry.Item;//May have receieved invalid item id, so added exception to throw

        PullQty(outputQty, item, out unfulfilled);
        InventoryDisplayManager.UpdateDisplayAll();

        return (outputQty, item);
    }

    public void PushItems(int id, int insertQty)
    {
        InventoryEntry matchingItem = Find(id);
        Item item = matchingItem != null ? matchingItem.Item : UnityEngine.Object.Instantiate(ItemDictionary.items[id]);
        
        PushQty(insertQty, item, out int remainder);
        InventoryDisplayManager.UpdateDisplayAll();
    }

    public override void ClearInventory()
    {
        base.ClearInventory();
        InventoryDisplayManager.UpdateDisplayAll();
    }

    public override void LoadFromStorage(List<InventoryEntry> storedData)
    {
        base.LoadFromStorage(storedData);
        InventoryDisplayManager.InitializeInventoryDisplay();
    }
}
