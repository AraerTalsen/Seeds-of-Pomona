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
        unfulfilled = requestedQty;
        Item item = null;

        int totalQty = Sum(id);
        //int outputQty = Mathf.Min(totalQty, requestedQty);

        if(Find(id) is InventoryEntry entry)
        {
            item = entry.Item;

            PullQty(requestedQty, item, out unfulfilled);
            DisplayManager.UpdateDisplayAll();
        }
        return (requestedQty - unfulfilled, item);
    }

    public void PushItems(int id, int insertQty, out int remainder, bool isUniqueInstance = false)
    {
        Item item = !isUniqueInstance ? ItemDictionary.items[id] : UnityEngine.Object.Instantiate(ItemDictionary.items[id]);
        
        if(isUniqueInstance) 
        {
            item.CloneKey = item.GetInstanceID().ToString(); 
            UniqueItemManager.Save(item.CloneKey, (PUp)item);
        }
        PushQty(insertQty, item, out remainder);
        
        DisplayManager.UpdateDisplayAll();
    }

    public abstract void Delete(int slotIndex);
    public abstract void ClearInventory();
    protected abstract void InitializeInventory();
}
