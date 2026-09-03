using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryEntry
{
    [SerializeField] private int quantity; 
    [SerializeField] private Item item; 
    [SerializeField] private string debugItemName;
    public int Quantity { get => quantity; private set => quantity = value; }
    public Item Item { get => item; private set => item = value; }

    public bool IsEmpty => Item == null || Quantity <= 0;

    public InventoryEntry(int qty, Item item)
    {
        ApplyState(qty, item);
    }
    private void UpdateDebug()
    {
        debugItemName = item == null ? "Empty" : item.name;
    }

    public static InventoryEntry Empty() => new(0, null);

    public int Set(int qty, Item item)
    {
        return ApplyState(qty, item);
    }

    public int TryAddQuantity(int delta)
    {
        int deficit = (delta > 0 ? Item.maxStackSize : 0) - Quantity;
        int contribution = delta > 0 ? Mathf.Min(delta, deficit) : Mathf.Max(delta, deficit);
        
        if(deficit != 0)
        {
            ApplyState(Quantity + contribution, Item);
        }
        return delta - contribution;
    }

    public void Remove()
    {
        Clear();
    }

    public InventoryEntry Clone() => new(Quantity, Item);

    private int ApplyState(int qty, Item item)
    {
        if(qty <= 0 || item == null)
        {
            Clear();
            return qty;
        }
        
        int contribution = qty > item.maxStackSize ? item.maxStackSize : qty;

        Quantity = contribution;
        Item = item;
        UpdateDebug();
        return qty - contribution;
    }

    private void Clear()
    {
        Quantity = 0;
        Item = null;
        UpdateDebug();
    }
}
