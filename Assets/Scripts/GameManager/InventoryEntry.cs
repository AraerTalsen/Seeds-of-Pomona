using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryEntry
{
    private int quantity;
    public Item item;

    [SerializeField]
    private int itemId;
    
    public int Quantity
    {
        get => quantity;
        set
        {
            quantity = value;
            if (quantity <= 0)
            {
                Clear();
            }
        }
    }

    public Item Item
    {
        get => item;
        set
        {
            item = value;
            itemId = item.id;
        }
    }

    public bool IsEmpty
    {
        get
        {
            bool isEmpty = item == null || quantity <= 0;
            return isEmpty;
        }
    }

    public InventoryEntry(int qty, Item itm)
    {
        quantity = qty;
        item = itm;
        ItemId = item != null ? item.id : -1;
    }

    public InventoryEntry(int qty)
    {
        quantity = qty;
    }

    public void Deconstruct(out int quantity, out Item item)
    {
        quantity = this.quantity;
        //item = this.item == null ? Object.Instantiate(ItemDictionary.items[itemId]) : item;
        item = this.item;
    }

    public InventoryEntry Clone()
    {
        return new InventoryEntry(quantity, item);
    }

    public int ItemId
    {
        set
        {
            itemId = value;
        }
    }

    //Perhaps make private, since setting qty to 0 calls this anyway
    public void Clear()
    {
        quantity = 0;
        item = null;
        itemId = -1;
    }
}
