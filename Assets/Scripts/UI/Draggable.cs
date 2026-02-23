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
    public static Draggable Instance { get; private set; }

    private void Awake()
    {
        TrySetInstance();
    }

    private void TrySetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    public void Update()
    {
        transform.position = Input.mousePosition;
    }

    public InventoryEntry SetDraggable(DragNDropInventory inv, int slotIndex, bool isPrivateInput, bool isSameType)
    {
        InventoryEntry temp = inventoryItem.Clone();
        InventoryEntry slotItem = inv.Read(slotIndex);
        print($"Darggable Incoming entry: {slotItem.Item}, {slotItem.Quantity}, Outgoing entry: {temp.Item}, {temp.Quantity}");

        if (isSameType)
        {

            if (isPrivateInput)
            {
                (slotItem, temp) = HandleSameItemType(inventoryItem.Quantity, slotItem.Quantity, slotItem.Item.maxStackSize, slotItem.Item);
                inv.SetSlot(temp.Quantity, temp.Item, slotIndex);
            }
            else
            {
                (temp, slotItem) = HandleSameItemType(slotItem.Quantity, inventoryItem.Quantity, slotItem.Item.maxStackSize, slotItem.Item);
                inv.SetSlot(temp.Quantity, temp.Item, slotIndex);
            }
        }

        inventoryItem.Set(slotItem.Quantity, slotItem.Item);
        inv.SetSlot(temp.Quantity, temp.Item, slotIndex);
        print($"Darggable  double check Incoming entry: {slotItem.Item}, {slotItem.Quantity}, Outgoing entry: {temp.Item}, {temp.Quantity}");

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
            print("Item is not null");
            refInventory = inv;
            currentSlot = slotIndex;

            image.color = new Color(255, 255, 255, 1);
            image.sprite = inventoryItem.Item.sprite;
            txt.text = inventoryItem.Quantity > 1 ? inventoryItem.Quantity.ToString() : "";
        }
        else 
        {
            print("Item is null");
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
