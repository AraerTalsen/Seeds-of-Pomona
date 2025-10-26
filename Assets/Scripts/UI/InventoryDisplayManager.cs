using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class InventoryDisplayManager : BasicMenu
{
    private Transform[][] slots;

    public Inventory Inventory { get; set; }

    public override void ToggleMenu(GameObject menu2 = null, Move_Player mp = null)
    {
        base.ToggleMenu(menu2);
    }

    public void InitializeInventoryDisplay()
    {
        int numInventories = ActiveMenu.transform.GetChild(0).childCount;
        slots = new Transform[numInventories][];

        for (int i = 0; i < numInventories; i++)
        {
            slots[i] = ActiveMenu.transform.GetChild(0).GetChild(i).Cast<Transform>().ToArray();
            int numSlots = slots[i].Length;

            for (int j = 0; j < numSlots; j++)
            {
                Transform t = slots[i][j];
                InventorySlot iSlot = t.GetComponent<InventorySlot>();
                iSlot.refInvScript = Inventory;
                iSlot.invIndex = i;
                iSlot.slotIndex = j;
            }
        }
        UpdateDisplayAll();
    }

    public void UpdateItemDisplay(int slotIndex, int invIndex)
    {
        Transform t = slots[invIndex][slotIndex];
        Image slotImage = t.GetChild(0).GetComponent<Image>();
        TMP_Text txt = t.GetChild(0).GetChild(0).GetComponent<TMP_Text>();

        bool slotIsUsed = Inventory.inventories[invIndex][slotIndex] != null && !Inventory.inventories[invIndex][slotIndex].IsEmpty;
        txt.text = slotIsUsed ? Inventory.inventories[invIndex][slotIndex].Quantity.ToString() : "";

        if (slotIsUsed && slotImage.sprite == null)
        {
            slotImage.sprite = Inventory.inventories[invIndex][slotIndex].item.sprite;
        }
        else if (!slotIsUsed && slotImage.sprite != null)//This check shouldn't be needed, just using else, without a condition, should work, but just in case
        {
            slotImage.sprite = null;
        }
    }
    
    public void UpdateDisplayAll()
    {
        for (int i = 0; i < Inventory.inventories.Length; i++)
        {
            for (int j = 0; j < Inventory.inventories[i].Length; j++)
            {
                UpdateItemDisplay(j, i);
            }
        }
    }
}
