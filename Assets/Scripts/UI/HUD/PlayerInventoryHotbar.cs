using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryHotbar : BoundedDDI
{
    private int selectionInput = 0;
    public int SelectionInput 
    { 
        get => selectionInput;
        set
        {
            selectionInput = value;
            UpdateSelectionReticle();
        }
    }
    public Item CurrentSelection => Entries[SelectionInput].Item;
    private Transform selectionReticle;
    private Transform hudSlots;
    public PlayerInventoryHotbar(Transform invContainer, Transform hudContainer) : base(invContainer, true)
    {
        hudSlots = hudContainer.GetChild(0);
        selectionReticle = hudContainer.GetChild(1);
        Listener?.SubscribeToChanges(UpdateAllSlots, InventoryListener.SlotTouchMode.Pulled);
        Listener?.SubscribeToChanges(UpdateAllSlots, InventoryListener.SlotTouchMode.Pushed);
        Listener?.SubscribeToChanges(UpdateAllSlots, InventoryListener.SlotTouchMode.Set);
    }

    private void UpdateAllSlots()
    {
        for(int i = 0; i < Count; i++)
        {
            UpdateHUDSlots(i);
        }
    }

    private void UpdateHUDSlots(int index)
    {
        Transform slot = hudSlots.GetChild(index);
        Image image = slot.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI qty = slot.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        image.sprite = Read(index).IsEmpty ? null : Read(index).Item.sprite;
        qty.text = Read(index).IsEmpty ? "" : Read(index).Quantity > 1 ? Read(index).Quantity.ToString() : "";
    }

    private void UpdateSelectionReticle()
    {
        RectTransform rect = selectionReticle.GetComponent<RectTransform>();
        Vector2 current = rect.anchoredPosition;
        rect.anchoredPosition = new(SelectionInput * 60, current.y);
    }

    public override void LoadFromStorage(List<InventoryEntry> storedData)
    {
        base.LoadFromStorage(storedData);
        UpdateSelectionReticle();
        UpdateAllSlots();
    }

    public void TryUseTool(EffectContext context)
    {
        if(CurrentSelection is Tool tool)
        {
            tool.ToolAction(context);
            SetSlot(Read(SelectionInput).Quantity - 1, CurrentSelection, SelectionInput);
        }
    }
}
