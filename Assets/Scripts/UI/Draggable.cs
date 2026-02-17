using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Draggable : MonoBehaviour
{
    [SerializeField] private GameObject toolTipDisplay;
    [SerializeField] private TextMeshProUGUI toolTipText;
    public PlayerInventory pi;
    public Image image;

    private InventoryEntry inventoryItem = InventoryEntry.Empty();
    private int currentSlot;
    private ItemInventory refInventory;

    public InventoryEntry InventoryItem => inventoryItem;
    public bool IsDisplayOpen => toolTipDisplay.activeSelf;

    public void Update()
    {
        transform.position = Input.mousePosition;
    }

    //Could call inv.SetSlot(temp, invIndex, slotIndex); just once after all of the conditionals
    public InventoryEntry SetDraggable(DragNDropInventory inv, int slotIndex, int invIndex, bool isPrivateInput, bool isSameType)
    {
        InventoryEntry temp = inventoryItem;
        InventoryEntry slotItem = inv.Read(slotIndex);

        if (isSameType)
        {

            if (isPrivateInput)
            {
                (inventoryItem, temp) = HandleSameItemType(inventoryItem.Quantity, slotItem.Quantity, slotItem.Item.maxStackSize, slotItem.Item);
                inv.SetSlot(temp, slotIndex);
            }
            else
            {
                (temp, inventoryItem) = HandleSameItemType(slotItem.Quantity, inventoryItem.Quantity, slotItem.Item.maxStackSize, slotItem.Item);
                inv.SetSlot(temp, slotIndex);
            }
        }
        else
        {
            inventoryItem = slotItem;
            inv.SetSlot(temp, slotIndex);
        }

        SetDraggableDisplay(inv, slotIndex);


        return temp;
    }

    private (InventoryEntry, InventoryEntry) HandleSameItemType(int recievingItemQty, int losingItemQty, int maxStackSize, Item item)
    {
        (int gainedQty, int lostQty) = AddQuantityToItem(recievingItemQty, losingItemQty, maxStackSize);
        InventoryEntry gainingInventory = new(gainedQty, item);
        InventoryEntry losingInventory = lostQty == 0 ? InventoryEntry.Empty() : new(lostQty, item);
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

    private void SetDraggableDisplay(DragNDropInventory inv, int slotIndex)
    {
        TMP_Text txt = transform.GetChild(0).GetComponent<TMP_Text>();
        if(inventoryItem != null && !inventoryItem.IsEmpty) 
        {
            refInventory = inv;
            currentSlot = slotIndex;

            image.color = new Color(255, 255, 255, 1);
            image.sprite = inventoryItem.Item.sprite;
            txt.text = inventoryItem.Quantity > 1 ? inventoryItem.Quantity.ToString() : "";
        }
        else 
        {
            image.color = new Color(255, 255, 255, 0);
            image.sprite = null;
            txt.text = "";
        }
    }

    public void OpenToolTipDisplay(string toolTip)
    {
        toolTipText.text = toolTip;
        toolTipDisplay.SetActive(true);
    }

    public void CloseToolTipDisplay() => toolTipDisplay.SetActive(false);
}
