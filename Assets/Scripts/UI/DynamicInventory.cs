using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicInventory : ItemInventory
{
    /*public void Create(int qty, Item item) => _Inventory.Add(new InventoryEntry(qty, item));
    public void Create(InventoryEntry entry) => _Inventory.Add(entry);

    public override void Delete(int slotIndex) =>_Inventory.RemoveAt(slotIndex);

    protected override void UpdateQty(int qty, Item item, out int remainder)
    {
        base.UpdateQty(qty, item, out remainder);
        Append(remainder, item);
    }

    private void Append(int qty, Item item)
    {
        int stacks = qty / item.maxStackSize;
        int remainder = qty % item.maxStackSize;

        for(int i = 0; i < stacks; i++)
        {
            Create(item.maxStackSize, item);
        }

        Create(remainder, item);
    }*/
}
