using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float toolTipDelay = 0.25f;
    [SerializeField] private string toolTip;
    [SerializeField] private bool isTrashable = false;
    [SerializeField] private GameObject trashItem;
    public DragNDropInventory refInvScript;
    public int slotIndex;
    public bool isPrivateInput;
    public List<Item.ItemCategory> whitelist = new();
    public bool isLocked = false;
    public string ToolTip { get => toolTip; set => toolTip = value; }
    public float ToolTipDelay { get => toolTipDelay; set => toolTipDelay = value; }
    public bool IsTrashable { get => isTrashable; set => isTrashable = value; }
    private bool pointerHovering = false;
    private bool preparingToolTip = false;
    private Draggable draggable;

    private void Start()
    {
        draggable = Draggable.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            //The logic is bloated. How can this be broken up or simplified?
            bool isDNull = draggable.InventoryItem == null || draggable.InventoryItem.IsEmpty;
            bool isINull = refInvScript.Read(slotIndex) == null || refInvScript.Read(slotIndex).IsEmpty;
            Item dItem = !isDNull ? draggable.InventoryItem.Item : null;
            Item iItem = !isINull ? refInvScript.Read(slotIndex).Item : null;
            bool isSameObjType = dItem != null && iItem != null && dItem.id == iItem.id;
            bool hasWhitelist = whitelist.Count > 0;
            bool itemAllowedByWhitelist = hasWhitelist && !isDNull && draggable.InventoryItem.Item.categories
            .Any(c1 => whitelist
            .Any(c2 => c2 == c1));

            if(!isLocked && !((isPrivateInput && !isSameObjType && !isDNull) || hasWhitelist && !itemAllowedByWhitelist && !isDNull))
            {
                InventoryEntry temp = draggable.SetDraggable(refInvScript, slotIndex, isPrivateInput, isSameObjType);
            
                transform.GetChild(0).GetComponent<Image>().sprite = temp != null && !temp.IsEmpty ? temp.Item.sprite : null;
                TMP_Text text = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
                text.text = temp != null && !temp.IsEmpty && temp.Quantity > 1 ? temp.Quantity.ToString() : "";
            }
            UpdateToolTipDisplay();
        }
    }

    private void UpdateToolTipDisplay()
    {
        if(pointerHovering && !refInvScript.Read(slotIndex).IsEmpty)
        {
            SetToolTipDisplay();
        }
        else
        {
            CloseToolTipDisplay();
        }
    }

    private void SetToolTipDisplay()
    {
        InventoryEntry entry = refInvScript.Read(slotIndex);
        string tempToolTip = !entry.IsEmpty && entry.Item.CurrentToolTip.CompareTo("") != 0 ? entry.Item.CurrentToolTip : ToolTip;
        
        if(tempToolTip.CompareTo("") != 0)
        {
            draggable.OpenToolTipDisplay(tempToolTip);
        }
    }
    private void CloseToolTipDisplay()
    {
        if(draggable.IsDisplayOpen)
        {
            draggable.CloseToolTipDisplay();
        }

        if(preparingToolTip)
        {
            StopCoroutine(nameof(DelayToolTip));
            preparingToolTip = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerHovering = true;
        StartCoroutine(nameof(DelayToolTip));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerHovering = false;
        CloseToolTipDisplay();
        if(isTrashable)
        {
            trashItem.SetActive(false);
        }
    }

    public void DeleteItem()
    {
        refInvScript.Delete(slotIndex);
    }

    private IEnumerator DelayToolTip()
    {
        preparingToolTip = true;
        yield return new WaitForSeconds(toolTipDelay);
        preparingToolTip = false;
        SetToolTipDisplay();
        if(isTrashable)
        {
           trashItem.SetActive(true); 
        }
    }
}
