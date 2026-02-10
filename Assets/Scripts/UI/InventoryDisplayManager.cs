using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class InventoryDisplayManager
{
    //private Transform[][] slots;
    private Transform[] slots;

    public InventoryDisplayManager
    (DragNDropInventory inv, Transform invContainer)
    {
        Inventory = inv;
        InvContainer = invContainer;
        InitializeInventoryDisplay();
    }

    private DragNDropInventory Inventory { get; set; }
    private Transform InvContainer { get; set; }
    public BasicMenu Menu { get; set; }

    public void InitializeInventoryDisplay()
    {
        //int numInventories = ActiveMenu.transform.GetChild(0).childCount;
        //slots = new Transform[numInventories][];;

        //for (int i = 0; i < numInventories; i++)
        //{
            //slots[i] = ActiveMenu.transform.GetChild(0).GetChild(i).Cast<Transform>().ToArray();
            slots = InvContainer.Cast<Transform>().ToArray();
            int numSlots = slots.Length;

            for (int i = 0; i < numSlots; i++)
            {
                Transform t = slots[i];
                InventorySlot iSlot = t.GetComponent<InventorySlot>();
                iSlot.refInvScript = Inventory;
                //iSlot.invIndex = i;
                iSlot.slotIndex = i;
            }
        //}
        UpdateDisplayAll();
    }

    public void UpdateDisplayAll()
    {
        //for (int i = 0; i < Inventory.inventories.Length; i++)
        //{
            for (int i = 0; i < Inventory.Entries/*.inventories[i]*/.Count; i++)
            {
                UpdateItemDisplay(i);
            }
        //}
    }

    public void UpdateItemDisplay(int slotIndex)
    {
        Transform t = slots[slotIndex];
        Image slotImage = t.GetChild(0).GetComponent<Image>();
        TMP_Text txt = t.GetChild(0).GetChild(0).GetComponent<TMP_Text>();

        bool slotIsUsed = Inventory.Read(slotIndex) != null && !Inventory.Read(slotIndex).IsEmpty;
        txt.text = slotIsUsed ? Inventory.Read(slotIndex).Quantity.ToString() : "";

        if (slotIsUsed && slotImage.sprite == null)
        {
            slotImage.sprite = Inventory.Read(slotIndex).Item.sprite;
        }
        else if (!slotIsUsed && slotImage.sprite != null)//This check shouldn't be needed, just using else, without a condition, should work, but just in case
        {
            slotImage.sprite = null;
        }
    }
}
