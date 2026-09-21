using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerInventoryManager : PersistentObject<PlayerInventoryData>
{
    [SerializeField] private PlayerInventoryData persist;
    [SerializeField] private Transform bagContainer, powerupHUDContainer, powerupContainer, hotbarContainer, hotbarHUDContainer;
    [SerializeField] private GameObject powerupHUDSlotPrefab;
    [SerializeField] private FlexInvDisplayManager.ISlotPrefill prefill;
    [SerializeField] private Wallet wallet;
    [SerializeField] private int powerupCapacity;
    private BoundedDDI bag;
    private PlayerInventoryHotbar hotbar;
    private PowerUpDisplay powerupSlots;
    private PowerUpHelper powerupHelper;

    public Wallet Wallet => wallet;
    public PlayerInventoryHotbar Hotbar => hotbar;
    public int HotbarSize => hotbar.Entries.Count;
    public int HotbarSelection { get => hotbar.SelectionInput; set => hotbar.SelectionInput = value; }
    public bool HasDied { get; set; }

    protected override void PullData()
    {
        bag = new(bagContainer);
        hotbar = new(hotbarContainer, hotbarHUDContainer);
        powerupSlots = new(powerupCapacity, powerupContainer, prefill, powerupHUDSlotPrefab, powerupHUDContainer, powerupHelper);
        //boonProfile = new(props, stats.Stats);

        if (!Persist.IsPersisting)
        {
            Persist.ClearInventory();
            Persist.ClearPowerups();
            Persist.ClearHotbar();
            Persist.LockStates.Clear();
            
            //Persist.Boons = boonProfile.Modifiers;
            Persist.IsPersisting = true;
        }
        else
        {
            bag.LoadFromStorage(Persist.LoadInventory());
            hotbar.LoadFromStorage(Persist.LoadHotbar());
            powerupSlots.RebuildSlots(Persist.LoadPowerups(), Persist.LockStates);
            //boonProfile.LoadModifiers(persist.Boons);
            wallet.CurrentBalance = Persist.Balance;
        }

        HasDied = Persist.HasDied;
    }

    protected override void PushData()
    {
        if(SceneManager.GetActiveScene().name.CompareTo("Wilderness") == 0)
        {
            TimerObserver.Instance.Broadcast();
        }
        
        Persist.Balance = wallet.CurrentBalance;
        Persist.SaveInventory(bag.Entries);
        Persist.SaveHotbar(hotbar.Entries);
        Persist.SavePowerups(powerupSlots.Entries);

        List<bool> lockStates = new();
        for(int i = 0; i < powerupSlots.Count; i++)
        {
            lockStates.Add(powerupSlots.IsSlotLocked(i));
        }
        Persist.LockStates = lockStates;
    }

    public void Initialize(PowerUpHelper helper, Func<EffectContext> factory)
    {
        powerupHelper = helper;
        powerupHelper.Context = factory;
        Persist = RetrieveData(persist);
        PullData();
    }

    public void Tick() => powerupSlots.PowerupInterface();

    public void OnDisable()
    {
        PushData();
    }

    public void PushItems(int id, int quantity, out int remainder, bool isUniqueInstance = false)
    {
        hotbar.PushItems(id, quantity, out remainder, isUniqueInstance);
        bag.PushItems(id, remainder, out remainder, isUniqueInstance);
    }
    public (int qty, Item item) PullItems(int id, int requestedQty, out int unfulfilled)
    {
        Item item = ItemDictionary.items[id];
        (int qty1, _) = bag.PullItems(id, requestedQty, out unfulfilled);
        (int qty2, _) = hotbar.PullItems(id, unfulfilled, out unfulfilled);
        return (qty1 + qty2, item);
    }
    public InventoryEntry Find(Item item)
    {
        InventoryEntry entry = bag.Find(item);
        if(entry == null || entry.IsEmpty)
        {
            entry = hotbar.Find(item);
        }

        return entry;
    }
    public InventoryEntry Find(int id)
    {
        InventoryEntry entry = bag.Find(id);
        if(entry == null || entry.IsEmpty)
        {
            entry = hotbar.Find(id);
        }

        return entry;
    }
    public InventoryEntry Find(Item.ItemCategory category)
    {
        InventoryEntry entry = bag.Find(category);
        if(entry == null || entry.IsEmpty)
        {
            entry = hotbar.Find(category);
        }

        return entry;
    }
    public int Sum(Item item) => hotbar.Sum(item) + bag.Sum(item);
    public int Sum(int id) => hotbar.Sum(id) + bag.Sum(id);
    public void ClearInventory()
    {
        hotbar.ClearInventory();
        bag.ClearInventory();
        powerupSlots.ClearInventory();
    }
}
