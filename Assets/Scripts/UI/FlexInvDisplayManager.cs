using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FlexInvDisplayManager : InventoryDisplayManager
{
    [System.Serializable]
    public struct ISlotPrefill
    {
        [SerializeField] private bool isPrivateInput;
        [SerializeField] private bool isLocked;
        [SerializeField] private string toolTip;
        [SerializeField] private bool isTrashable;
        [SerializeField] private List<Item.ItemCategory> whitelist;
        [SerializeField] private GameObject iSlot;

        public bool IsPrivateInput => isPrivateInput;
        public bool IsLocked => isLocked;
        public string ToolTip => toolTip;
        public bool IsTrashable => isTrashable;
        public List<Item.ItemCategory> Whitelist => whitelist;
        public GameObject ISlot => iSlot;
    }
    
    private ISlotPrefill prefill;
    public int SlotCount => slots.Count;

    public FlexInvDisplayManager(DragNDropInventory inv, Transform invContainer, ISlotPrefill prefill) : base(inv, invContainer)
    {
        this.prefill = prefill;
    }

    public void AddNewISlot()
    {
        GameObject g = Object.Instantiate(prefill.ISlot, InvContainer);
        InventorySlot iSlotProps = g.GetComponent<InventorySlot>();

        iSlotProps.isPrivateInput = prefill.IsPrivateInput;
        iSlotProps.isLocked = prefill.IsLocked;
        iSlotProps.ToolTip = prefill.ToolTip;
        iSlotProps.IsTrashable = prefill.IsTrashable;
        iSlotProps.whitelist = prefill.Whitelist;
        iSlotProps.refInvScript = Inventory;
        iSlotProps.slotIndex = slots.Count;
        slots.Add(g.transform);

        UpdateItemDisplay(iSlotProps.slotIndex);
    }

    public void RemoveSlotAt(int index)
    {
        GameObject g = slots[index].gameObject;
        slots.RemoveAt(index);

        Object.Destroy(g);
        UpdateSlotIndex(index);
    }

    private void UpdateSlotIndex(int startIndex)
    {
        for(int i = startIndex; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();
            slot.slotIndex = i;
        }
    }
}
