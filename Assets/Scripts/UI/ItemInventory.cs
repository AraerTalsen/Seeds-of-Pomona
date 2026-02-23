using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ItemInventory : InventoryBase
{
    public InventoryListener Listener { get; set; }
    public int Sum(Item item)
    {
        List<int> locations = FindAll(item);
        int sum = 0;

        for(int i = 0; i < locations.Count; i++)
        {
            sum += Entries[locations[i]].Quantity;
        }

        return sum;
    }
    public int Sum(int id)
    {
        List<int> locations = FindAll(id);
        int sum = 0;

        for(int i = 0; i < locations.Count; i++)
        {
            sum += Entries[locations[i]].Quantity;
        }

        return sum;
    }

    public void PushQty(int qty, Item item, out int remainder)
    {
        UpdateQty(qty, item, out remainder);
    }

    public void PullQty(int qty, Item item, out int remainder)
    {
        UpdateQty(-qty, item, out remainder);
    }
    
    protected virtual void UpdateQty(int qty, Item item, out int remainder)
    {
        List<int> locations = FindAll(item);
        if(qty > 0)
        {
            locations.AddRange(FindAll(null));
        }

        if(qty != 0 && locations.Count > 0 && Listener != null)
        {
            Listener.StartNewEvent();
        }

        remainder = qty;
        int i = 0;
        while(remainder != 0 && i < locations.Count)
        {
            Insert(remainder, item, locations[i], out remainder);
            i++;
        }
    }

    private void Insert(int qty, Item item, int slotIndex, out int remainder)
    {
        if(Listener != null) 
        { Listener.TempItem = Read(slotIndex).Item; }

        remainder = qty;
        InventoryEntry slot = Entries[slotIndex];

        if(slot != null)
        {
            if(slot.IsEmpty)
            {
                remainder = SetItem(qty, item, slotIndex);
            }
            else if(item.id == slot.Item.id)
            {
                remainder = TryAddQuantity(qty, slotIndex);
            }
            
            Listener?.TouchSlot(slotIndex, Listener.TempItem, qty - remainder);
        }
    }
}
