using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class InventoryDisplayManager
{
    //private Transform[][] slots;
    protected List<Transform> slots;

    public InventoryDisplayManager (DragNDropInventory inv, Transform invContainer)
    {
        Inventory = inv;
        InvContainer = invContainer;
        InitializeInventoryDisplay();
    }

    public DragNDropInventory Inventory { get; set; }
    protected Transform InvContainer { get; set; }
    public BasicMenu Menu { get; set; }

    public void InitializeInventoryDisplay()
    {
        slots = InvContainer.Cast<Transform>().ToList();
        int numSlots = slots.Count;

        for (int i = 0; i < numSlots; i++)
        {
            Transform t = slots[i];
            InventorySlot iSlot = t.GetComponent<InventorySlot>();
            iSlot.refInvScript = Inventory;
            iSlot.slotIndex = i;
        }
        UpdateDisplayAll();
    }

    public void UpdateDisplayAll()
    {
        for (int i = 0; i < Inventory.Entries.Count; i++)
        {
            UpdateItemDisplay(i);
        }
    }

    public void UpdateItemDisplay(int slotIndex)
    {
        Transform t = slots[slotIndex];
        Image slotImage = t.GetChild(0).GetComponent<Image>();
        TMP_Text txt = t.GetChild(0).GetChild(0).GetComponent<TMP_Text>();

        bool slotIsUsed = Inventory.Read(slotIndex) != null && !Inventory.Read(slotIndex).IsEmpty;
        int qty = slotIsUsed ? Inventory.Read(slotIndex).Quantity : -1;
        txt.text = slotIsUsed && qty > 1 ? qty.ToString() : "";

        if (slotIsUsed && slotImage.sprite == null)
        {
            slotImage.sprite = Inventory.Read(slotIndex).Item.sprite;
        }
        else if (!slotIsUsed && slotImage.sprite != null)//This check shouldn't be needed, just using else, without a condition, should work, but just in case
        {
            slotImage.sprite = null;
        }
    }

    public void ToggleLock(int slotIndex)
    {
        InventorySlot slot = slots[slotIndex].gameObject.GetComponent<InventorySlot>();
        slot.isLocked = !slot.isLocked;
    }

    public bool IsSlotLocked(int slotIndex) => slots[slotIndex].gameObject.GetComponent<InventorySlot>().isLocked;

    public void SetSlotLock(int slotIndex, bool isLocked) => slots[slotIndex].gameObject.GetComponent<InventorySlot>().isLocked = isLocked;
}
