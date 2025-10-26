using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Draggable : MonoBehaviour
{
    public PlayerInventory pi;
    public Image image;

    private InventoryEntry inventoryItem = new(0);
    private int currentSlot;
    private Inventory refInventory;

    public InventoryEntry InventoryItem
    {
        get
        {
            return inventoryItem;
        }
    }

    public void Update()
    {
        transform.position = Input.mousePosition;
    }

    //Could call inv.SetSlot(temp, invIndex, slotIndex); just once after all of the conditionals
    public InventoryEntry SetDraggable(Inventory inv, int slotIndex, int invIndex, bool isPrivateInput, bool isSameType)
    {
        InventoryEntry temp = inventoryItem;
        InventoryEntry slotItem = inv.PullSlot(invIndex, slotIndex);

        if (isSameType)
        {

            if (isPrivateInput)
            {
                (inventoryItem, temp) = HandleSameItemType(inventoryItem.Quantity, slotItem.Quantity, slotItem.item.maxStackSize, slotItem.item);
                inv.SetSlot(temp, invIndex, slotIndex);
            }
            else
            {
                (temp, inventoryItem) = HandleSameItemType(slotItem.Quantity, inventoryItem.Quantity, slotItem.item.maxStackSize, slotItem.item);
                inv.SetSlot(temp, invIndex, slotIndex);
            }
        }
        else
        {
            inventoryItem = slotItem;
            inv.SetSlot(temp, invIndex, slotIndex);
        }

        SetDraggableDisplay(inv, slotIndex);


        return temp;
    }

    private (InventoryEntry, InventoryEntry) HandleSameItemType(int recievingItemQty, int losingItemQty, int maxStackSize, Item item)
    {
        (int gainedQty, int lostQty) = AddQuantityToItem(recievingItemQty, losingItemQty, maxStackSize);
        InventoryEntry gainingInventory = new InventoryEntry(gainedQty, item);
        InventoryEntry losingInventory = lostQty == 0 ? new InventoryEntry(0) : new InventoryEntry(lostQty, item);
        return (gainingInventory, losingInventory);
    }
    
    private (int, int) AddQuantityToItem(int recievingItemQty, int losingItemQty, int itemStackCap)
    {
        if(recievingItemQty == itemStackCap)
        {
            return (losingItemQty, recievingItemQty);
        }
        else if(recievingItemQty + losingItemQty <= itemStackCap)
        {
            return (recievingItemQty + losingItemQty, 0);
        }
        else
        {
            int remainder = recievingItemQty + losingItemQty - itemStackCap;
            return (itemStackCap, remainder);
        }
    }

    private void SetDraggableDisplay(Inventory inv, int slotIndex)
    {
        TMP_Text txt = transform.GetChild(0).GetComponent<TMP_Text>();
        if(inventoryItem != null && !inventoryItem.IsEmpty) 
        {
            refInventory = inv;
            currentSlot = slotIndex;

            image.color = new Color(255, 255, 255, 1);
            image.sprite = inventoryItem.item.sprite;
            txt.text = inventoryItem.Quantity.ToString();
        }
        else 
        {
            image.color = new Color(255, 255, 255, 0);
            image.sprite = null;
            txt.text = "";
        }
    }
}
