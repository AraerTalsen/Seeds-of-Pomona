using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    private Draggable draggable;
    public Inventory refInvScript;
    public int invIndex;
    public int slotIndex;
    public bool isPrivateInput;
    public int whitelist = -1;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(draggable == null)
        {
            //We want to access the panel. What's a more dynamic way to achieve this?
            Transform uiGroup = transform.parent.parent.parent.parent.parent;
            draggable = uiGroup.GetChild(uiGroup.childCount - 1).GetComponent<Draggable>();
        }

        if(eventData.button == PointerEventData.InputButton.Left)
        {
            //The logic is bloated. How can this be broken up or simplified?
            bool isDNull = draggable.InventoryItem == null || draggable.InventoryItem.IsEmpty;
            bool isINull = refInvScript.inventories[invIndex][slotIndex] == null || refInvScript.inventories[invIndex][slotIndex].IsEmpty;
            Item dItem = !isDNull ? draggable.InventoryItem.item : null;
            Item iItem = !isINull ? refInvScript.inventories[invIndex][slotIndex].item : null;
            bool isSameObjType = dItem != null && iItem != null && dItem.id == iItem.id;
            bool hasWhitelist = whitelist != -1;
            bool incomingUsableInMachines = !isDNull && draggable.InventoryItem.item.usableInMachines != null;
            bool itemAllowedByWhitelist = hasWhitelist &&  incomingUsableInMachines && draggable.InventoryItem.item.usableInMachines.Contains(whitelist);

            if(!((isPrivateInput && !isSameObjType && !isDNull) || hasWhitelist && !itemAllowedByWhitelist && !isDNull))
            {
                InventoryEntry temp = draggable.SetDraggable(refInvScript, slotIndex, invIndex, isPrivateInput, isSameObjType);
            
                transform.GetChild(0).GetComponent<Image>().sprite = temp != null && !temp.IsEmpty ? temp.item.sprite : null;
                transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = temp != null && !temp.IsEmpty ? temp.Quantity.ToString() : "";
            }
        }
    }
}
