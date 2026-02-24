using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DragNDropInventory : ItemInventory
{
    protected InventoryDisplayManager DisplayManager { get; set; }
    protected int Capacity { get; set; }

    public virtual void SetSlot(int qty, Item item, int slotIndex)
    {
        Listener?.StartNewEvent();
        if(Listener != null)
        {
            Listener.TempItem = Read(slotIndex).Item;
        }
        
        SetItem(qty, item, slotIndex);

        Listener?.TouchSlot(slotIndex, Listener.TempItem, InventoryListener.SlotTouchMode.Set);
        DisplayManager.UpdateItemDisplay(slotIndex);
    }

    public (int qty, Item item) PullItems(int id, int requestedQty, out int unfulfilled)
    {
        int totalQty = Sum(id);
        int outputQty = Mathf.Min(totalQty, requestedQty);
        InventoryEntry entry = Find(id) ?? throw new ArgumentException($"Requested item of id: {id} does not exist in the inventory");
        Item item = entry.Item;

        PullQty(outputQty, item, out unfulfilled);
        DisplayManager.UpdateDisplayAll();

        return (outputQty, item);
    }

    public void PushItems(int id, int insertQty)
    {
        InventoryEntry matchingItem = Find(id);
        Item item = matchingItem != null ? matchingItem.Item : UnityEngine.Object.Instantiate(ItemDictionary.items[id]);
        
        PushQty(insertQty, item, out int remainder);
        DisplayManager.UpdateDisplayAll();
    }

    public abstract void Delete(int slotIndex);
    public abstract void ClearInventory();
    protected abstract void InitializeInventory();
}
