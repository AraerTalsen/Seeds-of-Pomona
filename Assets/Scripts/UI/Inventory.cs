using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Defines the number of inventories and item slots an entity has. Stores items
public abstract class Inventory : MonoBehaviour
{
    /*[SerializeField]
    private List<int> invSlotCount;

    public InventoryData persist;
    public InventoryEntry[][] inventories;
    public List<HeldItem> itemsHeld;

    private InventoryDisplayManager idm;

    protected InventoryDisplayManager DisplayManager
    {
        get => idm;
        set
        {
            idm = value;
            idm.Inventory = this;
            idm.InitializeInventoryDisplay();
        }
    }

    protected virtual void Start()
    {
        InitializeInvStorage();
        persist = RetrievePersistentData();
        InitializeSaveData(persist);
    }

    protected void InitializeInvStorage()
    {
        inventories = new InventoryEntry[invSlotCount.Count][];
        for (int i = 0; i < invSlotCount.Count; i++)
        {
            inventories[i] = new InventoryEntry[invSlotCount[i]];
            for (int j = 0; j < inventories[i].Length; j++)
            {
                inventories[i][j] = InventoryEntry.Empty();
            }
        }
    }

    protected InventoryData RetrievePersistentData()
    {
        InventoryData temp = (InventoryData)PersistentData.RetrieveDataContainer(persist.GetType().ToString());
        if (temp == null)
        {
            PersistentData.AddDataContainer(persist);
            return persist;
        }
        else
        {
            return temp;
        }
    }

    protected void InitializeSaveData(InventoryData persistentData)
    {
        //Currently, setting DisplayManager in Start throws errors when picking up items, likely due to Start overrides
        //in child classes. Once revising the child classes, return here to try and move DisplayManager initialization
        //to a more relevant location
        if (!persistentData.IsPersisting)
        {
            itemsHeld = new();

            persistentData.ItemsHeld = itemsHeld;
            persistentData.Inventories = inventories;
            DisplayManager = GetComponent<InventoryDisplayManager>();
        }
        else
        {
            itemsHeld = persistentData.ItemsHeld;
            inventories = persistentData.Inventories;
            DisplayManager = GetComponent<InventoryDisplayManager>();
            PullData();
        }
    }

    public void AddItem(int inputQty, int itemId, int invIndex)
    {
        (int, Item) item = (inputQty, Instantiate(ItemDictionary.items[itemId]));
        AddItem(item, invIndex);
    }

    //Try and store item in slots with existing stacks of the same item. If existing stacks are full, store item in empty slots
    public void AddItem((int qty, Item itm) item, int invIndex)
    {
        int holdIndex = GetHoldIndex(item.itm);

        item = DoesSlotTypeMatchItem(item, holdIndex, invIndex, false);
        if (item.qty > 0)
        {
            DoesSlotTypeMatchItem(item, holdIndex, invIndex, true);
        }
    }

    private (int, Item) DoesSlotTypeMatchItem((int qty, Item itm) item, int holdIndex, int invIndex, bool useEmptySlots)
    {
        for (int i = 0; i < inventories[invIndex].Length; i++)
        {
            bool isSlotEmpty = inventories[invIndex][i] == null || inventories[invIndex][i].IsEmpty;
            bool isEqual = (useEmptySlots && isSlotEmpty) || (!isSlotEmpty && inventories[invIndex][i].Item.id == item.itm.id);
            if (isEqual)
            {
                item = CheckRemainingSlotCapacity(item, holdIndex, invIndex, i, out bool shortCircuit);
                DisplayManager.UpdateItemDisplay(i, invIndex);
                if (shortCircuit || item.qty <= 0)
                {
                    break;
                }
            }
        }

        return item;
    }

    //Adds the greatest amount of a given item to a specified inventory slot that is allowed by the item's stack size
    private (int, Item) CheckRemainingSlotCapacity((int qty, Item itm) item, int holdIndex, int invIndex, int slotIndex, out bool shortCircuit)
    {
        int currentSlotQty = inventories[invIndex][slotIndex] == null || inventories[invIndex][slotIndex].IsEmpty ? 0 : inventories[invIndex][slotIndex].Quantity;
        bool isQtyLessThanMax = currentSlotQty + item.qty <= item.itm.maxStackSize;
        shortCircuit = false;

        if (inventories[invIndex][slotIndex].Quantity != item.itm.maxStackSize)
        {
            if (isQtyLessThanMax)
            {
                UpdateSlotQty(item.qty, holdIndex, invIndex, slotIndex, item.itm);
                shortCircuit = true;
                return (0, item.itm);
            }
            else
            {
                return InsertPartInSlot(item, holdIndex, invIndex, slotIndex);
            }
        }

        return item;
    }

    //Adds a portion less than the whole quantity of a given item to a specified inventory slot
    private (int, Item) InsertPartInSlot((int qty, Item itm) item, int holdIndex, int invIndex, int slotIndex)
    {
        bool isNotEmpty = inventories[invIndex][slotIndex] != null && !inventories[invIndex][slotIndex].IsEmpty;
        int qty = item.itm.maxStackSize - (isNotEmpty ? inventories[invIndex][slotIndex].Quantity : 0);

        UpdateSlotQty(qty, holdIndex, invIndex, slotIndex, item.itm);

        return (item.qty - qty, item.itm);
    }

    //Takes a positive or negative quantity and adds it to the existing slots quantity
    private void UpdateSlotQty(int insertQty, int holdIndex, int invIndex, int slotIndex, Item item)
    {
        if (inventories[invIndex][slotIndex].Item == null)
        {
            inventories[invIndex][slotIndex].item = item;
        }
        inventories[invIndex][slotIndex].TryAddQuantity(insertQty);

        bool isSlotEmpty = inventories[invIndex][slotIndex].IsEmpty;
        UpdateHeldItem(insertQty, isSlotEmpty, holdIndex, invIndex, slotIndex);
        DisplayManager.UpdateItemDisplay(slotIndex, invIndex);
    }

    public void UpdateSlotQty(int insertQty, int invIndex, int slotIndex, Item item)
    {
        if (inventories[invIndex][slotIndex].Item == null)
        {
            inventories[invIndex][slotIndex].Item = item;
        }
        inventories[invIndex][slotIndex].TryAddQuantity(insertQty);

        bool isSlotEmpty = inventories[invIndex][slotIndex].IsEmpty;
        UpdateHeldItem(insertQty, isSlotEmpty, FindItem(item.id), invIndex, slotIndex);
        DisplayManager.UpdateItemDisplay(slotIndex, invIndex);
    }

    public InventoryEntry Peek(int invIndex, int slotIndex)
    {
        return inventories[invIndex][slotIndex];
    }

    //Updates ongoing registry (heldItems) of item types, quantities, and slot coordinates in inventory 
    private void UpdateHeldItem(int insertQty, bool isSlotEmpty, int holdIndex, int invIndex, int slotIndex)
    {
        (int id, int oldQty, List<Vector2> oldCoords) = itemsHeld[holdIndex];

        int qty = oldQty + insertQty;

        if (qty > 0)
        {
            oldCoords = UpdateHeldItemCoords(oldCoords, insertQty, isSlotEmpty, invIndex, slotIndex);
            itemsHeld[holdIndex] = new HeldItem(id, qty, oldCoords);
        }
        else
        {
            itemsHeld.RemoveAt(holdIndex);
        }

    }
    
    //Do we need this else if? What purpose does it actually serve better than an else statement?
    private List<Vector2> UpdateHeldItemCoords(List<Vector2> coords, int insertQty, bool isSlotEmpty, int invIndex, int slotIndex)
    {
        Vector2 currentCoord = new(invIndex, slotIndex);

        if (isSlotEmpty)
        {
            coords.Remove(currentCoord);
        }
        else if (inventories[invIndex][slotIndex].Quantity == insertQty)
        {
            coords.Add(currentCoord);
            coords = SortCoords(coords);
        }

        return coords;
    }

    public void SetSlot(InventoryEntry entry, int invIndex, int slotIndex)
    {
        UpdateSlot(entry, false, invIndex, slotIndex);
    }


    public InventoryEntry PullSlot(int invIndex, int slotIndex)
    {
        InventoryEntry output = inventories[invIndex][slotIndex].Clone();

        UpdateSlot(output, true, invIndex, slotIndex);

        return output;
    }

    private void UpdateSlot(InventoryEntry entry, bool isClearing, int invIndex, int slotIndex)
    {
        if (isClearing)
        {
            inventories[invIndex][slotIndex].Remove();
        }
        else
        {
            inventories[invIndex][slotIndex] = entry;
        }

        if (entry != null && !entry.IsEmpty)
        {
            int mod = isClearing ? -1 : 1;
            UpdateHeldItem(mod * entry.Quantity, isClearing, GetHoldIndex(entry.Item), invIndex, slotIndex);
        }

        DisplayManager.UpdateItemDisplay(slotIndex, invIndex);
    }

    //Extract given quantity of an item from the inventory
    public InventoryEntry PullItems(int heldItmIndex, int requestedQty)
    {
        if (heldItmIndex != -1)
        {
            (_, int totalQty, List<Vector2> coords) = itemsHeld[heldItmIndex];

            int outputQty = totalQty < requestedQty ? totalQty : requestedQty;
            Item item = inventories[(int)coords[0].x][(int)coords[0].y].Item;

            InventoryEntry output = new(outputQty, item);

            DeleteItemsByAmnt(requestedQty, heldItmIndex, coords, totalQty, item);
            return output;
        }
        return InventoryEntry.Empty();
    }

    //Remove items from inventory storage, item registry, and display
    private void DeleteItemsByAmnt(int requestedQty, int heldItmIndex, List<Vector2> coords, int totalQty, Item item)
    {
        while (requestedQty > 0 && totalQty > 0)
        {
            int invIndex = (int)coords[0].x;
            int slotIndex = (int)coords[0].y;
            int currentSlotQty = inventories[invIndex][slotIndex].Quantity;
            int insertQty = requestedQty < currentSlotQty ? requestedQty : currentSlotQty;

            UpdateSlotQty(-insertQty, heldItmIndex, invIndex, slotIndex, item);
            requestedQty -= currentSlotQty;

            //DisplayManager.UpdateItemDisplay(slotIndex, invIndex);
        }
    }

    public int FindItem(int id)
    {
        for (int i = 0; i < itemsHeld.Count; i++)
        {
            if (itemsHeld[i].id == id)
            {
                return i;
            }
        }
        return -1;
    }

    //Returns the index in the item registry of the requested item. Add item to the registry if it doesn't exist yet and return that index
    private int GetHoldIndex(Item item)
    {
        int holdIndex = FindItem(item.id);
        if (holdIndex == -1)
        {
            itemsHeld.Add(new HeldItem(item.id, 0, new()));
            holdIndex = itemsHeld.Count - 1;
        }

        return holdIndex;
    }

    //Sort coords by from first to last inventory (if entity has more than one inventory) and slot indeces
    private List<Vector2> SortCoords(List<Vector2> coords)
    {
        Vector2 v = coords[^1];

        for (int i = 0; i < coords.Count; i++)
        {
            if (v.x <= coords[i].x && v.y <= coords[i].y)
            {
                coords.Insert(i, v);
                coords.RemoveAt(coords.Count - 1);
                break;
            }
        }

        return coords;
    }

    public void ClearInventory()
    {
        for(int i = 0; i < inventories.Length; i++)
        {
            for(int j = 0; j < inventories[i].Length; j++)
            {
                inventories[i][j].Remove();
            }
        }
        itemsHeld = new();

        persist.Inventories = inventories;
        persist.ItemsHeld = itemsHeld;
    }

    public abstract void PullData();*/
}