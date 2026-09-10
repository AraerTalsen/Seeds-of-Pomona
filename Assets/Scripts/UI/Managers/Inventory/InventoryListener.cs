using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryListener
{
    public enum SlotTouchMode
    {
        Read,
        Set,
        Pushed,
        Pulled,
        Created,
        Cleared,
        Deleted
    }

    private List<int> lastTouchedSlots = new();
    public List<int> LastTouchedSlots => lastTouchedSlots;
    private List<SlotTouchMode> slotChange = new();
    public List<SlotTouchMode> SlotChange => slotChange;
    private List<Item> slotsLastItems = new();
    public List<Item> SlotsLastItems => slotsLastItems;
    public Item TempItem { get; set; }
    public int InventorySizeDelta = 0;
    private Dictionary<int, Action> slotSubscriptionAlerts = new();
    private Dictionary<Item, Action> itemSubscriptionAlerts = new();
    private Dictionary<SlotTouchMode, Action> changeSubscriptionAlerts = new();
    private bool isSlotListCalled = false;

    public void StartNewEvent()
    {
        LastTouchedSlots.Clear();
        SlotsLastItems.Clear();
        SlotChange.Clear();
        InventorySizeDelta = 0;
        isSlotListCalled = false;
    }

    public void TouchSlot(int slotIndex, Item item, int qtyDelta)
    {
        TouchSlot(slotIndex, item, QtyDeltaToMode(qtyDelta));
    }
    
    public void TouchSlot(int slotIndex, Item item, SlotTouchMode change = SlotTouchMode.Read)
    {
        LastTouchedSlots.Add(slotIndex);
        SlotsLastItems.Add(item);
        SlotChange.Add(change);

        TryCallBacks(slotIndex, item, change);
    }

    private void TryCallBacks(int slotIndex, Item item, SlotTouchMode change)
    {
        TrySlotsCallback(slotIndex);
        TryItemsCallback(item);
        TryChangesCallback(change);
    }

    public void SubscribeToTouchedSlots(Action callBack, int slotIndex = -1)
    {
        if(slotSubscriptionAlerts.ContainsKey(slotIndex))
        {
            string listenerType = slotIndex == -1 ? "Slots List" : "Slot #" + slotIndex;
            Debug.Log("Overwriting subscription alert for " + listenerType);
        }

        slotSubscriptionAlerts[slotIndex] = callBack;
    }

    public void UnsubscribeFromTouchedSlots(int slotIndex = -1)
    {
        if(slotSubscriptionAlerts.ContainsKey(slotIndex))
        {
            slotSubscriptionAlerts.Remove(slotIndex);
        }
    }

    private void TrySlotsCallback(int slotIndex)
    {
        foreach(KeyValuePair<int, Action> pair in slotSubscriptionAlerts)
        {
            if(!isSlotListCalled && pair.Key == -1)
            {
                isSlotListCalled = true;
                pair.Value();
            }
            else if(pair.Key == slotIndex)
            {
                pair.Value();
                break;
            }
        }
    }

    public void SubscribeToTouchedItems(Action callBack, Item item)
    {
        if(itemSubscriptionAlerts.ContainsKey(item))
        {
            Debug.Log("Overwriting subscription alert for " + item);
        }

        itemSubscriptionAlerts[item] = callBack;
    }

    public void UnsubscribeFromTouchedItems(Item item)
    {
        if(itemSubscriptionAlerts.ContainsKey(item))
        {
            itemSubscriptionAlerts.Remove(item);
        }
    }

    private void TryItemsCallback(Item item)
    {
        foreach(KeyValuePair<Item, Action> pair in itemSubscriptionAlerts)
        {
            if(pair.Key.id == item.id)
            {
                pair.Value();
                break;
            }
        }
    }

    public void SubscribeToChanges(Action callBack, SlotTouchMode change)
    {
        if(changeSubscriptionAlerts.ContainsKey(change))
        {
            Debug.Log("Overwriting subscription alert for " + change);
        }

        changeSubscriptionAlerts[change] = callBack;
    }
    
    public void UnsubscribeFromChanges(SlotTouchMode change)
    {
        if(changeSubscriptionAlerts.ContainsKey(change))
        {
            changeSubscriptionAlerts.Remove(change);
        }
    }

    private void TryChangesCallback(SlotTouchMode change)
    {
        foreach(KeyValuePair<SlotTouchMode, Action> pair in changeSubscriptionAlerts)
        {
            if(pair.Key == change)
            {
                pair.Value();
                break;
            }
        }
    }

    public void PrintSlots()
    {
        Debug.Log("---------The following slots have been touched in the most recent inventory access---------");
        foreach (int index in LastTouchedSlots)
        { 
            Debug.Log($"Slot {index}"); 
        }
    }

    public void PrintSlotDetails(int slotIndex)
    {
        int index = LastTouchedSlots.FindIndex( i => i == slotIndex);
        
        if(index > -1)
        {
            Debug.Log($"Slot #{slotIndex} had a{(SlotsLastItems[index] == null ? "n empty slot" : " " + SlotsLastItems[index])} in it and was {SlotChange[index]}");
        }
        else
        {
            Debug.Log($"Listener did not detect Slot #{slotIndex}");
        }
    }

    public void PrintAllDetails()
    {
        for(int i = 0; i < LastTouchedSlots.Count; i++)
        {
            PrintSlotDetails(LastTouchedSlots[i]);
        }
    }

    private SlotTouchMode QtyDeltaToMode(int qtyDelta) =>
        qtyDelta switch
        {
            > 0 => SlotTouchMode.Pushed,
            < 0 => SlotTouchMode.Pulled,
            _   => SlotTouchMode.Read
        };  
}
