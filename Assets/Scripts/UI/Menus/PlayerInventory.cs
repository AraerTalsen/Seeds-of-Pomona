using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInventory : BasicMenu
{    
    //[SerializeField] private FlexInvDisplayManager.ISlotPrefill prefill;
    //[SerializeField] private BoonDisplay.BoonDisplayProps props;
    //[SerializeField] private Transform bagContainer, powerupContainer, hotbarContainer, hotbarHUDContainer;
    [SerializeField][TextArea] private string deathMsg;
    //[SerializeField] private GameObject HUDSlot;
    //[SerializeField] private Transform HUDContainer;
    [SerializeField] private Move_Player move_Player;
    //public PlayerInventoryData persist;
    //public int bagCapacity, powerupCapacity;
    //public Wallet wallet;
    //private BoundedDDI bag;
    //private PlayerInventoryHotbar hotbar;
    //private PowerUpDisplay powerupSlots;
    //private BoonProfile boonProfile;
    [SerializeField] private EntityStats stats;
    [SerializeField] private EntityOrientation orientation;
    //private PowerUpHelper powerupHelper;
    [SerializeField] private TactileSense tactileSense;
    public TactileSense TactileSense => tactileSense;
    public EntityStats Stats => stats;
    [SerializeField] private PlayerInventoryManager manager;
    public Wallet Wallet => manager.Wallet;

    private void Start()
    {
        manager.Initialize(GetComponent<PowerUpHelper>(), CreateEffectContext);
        if(manager.HasDied)
        {
            LogDeathMessage();
        }
        /*powerupHelper = GetComponent<PowerUpHelper>();
        powerupHelper.Context = CreateEffectContext;
        Persist = RetrieveData(persist);
        PullData();*/
    }

    public EffectContext CreateEffectContext()
    {
        return new()
        {
          Owner = new()
          {
            Body = gameObject,
            Worldbox = transform.GetChild(0).GetChild(0).gameObject,
            Stats = stats.StatBlock,
            Orientation = orientation,
            TactileSense = TactileSense
          }
        };
    }

    private void Update()
    {
        manager.Tick();
        CheckIfControlUse();
    }

    /*protected override void PullData()
    {
        bag = new(bagContainer);
        hotbar = new(hotbarContainer, hotbarHUDContainer);
        powerupSlots = new(powerupCapacity, powerupContainer, prefill, HUDSlot, HUDContainer, powerupHelper);
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

        if (Persist.HasDied)
        {
            LogDeathMessage();
        }
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
    }*/

    public void PushItems(int id, int quantity, out int remainder, bool isUniqueInstance = false) => 
        manager.PushItems(id, quantity, out remainder, isUniqueInstance);
    public (int qty, Item item) PullItems(int id, int requestedQty, out int unfulfilled) => manager.PullItems(id, requestedQty, out unfulfilled);
    public InventoryEntry Find(int id) => manager.Find(id);
    public InventoryEntry Find(Item item) => manager.Find(item);
    public InventoryEntry Find(Item.ItemCategory category) => manager.Find(category);
    public int Sum(int id) => manager.Sum(id);
    public int Sum(Item item) => manager.Sum(item);
    public void ClearInventory() => manager.ClearInventory();

    public void TriggerDeath()
    {
        manager.HasDied = true;
    }

    private void LogDeathMessage()
    {
        manager.HasDied = false;
        TextWindowManager.Instance.SetMessage(deathMsg, move_Player);
    }

    private void CheckIfControlUse()
    {
        SelectFromHotbar();
        TryUseTool();
    }
    
    private void SelectFromHotbar()
    {
        KeySelect();
        ScrollSelect();
    }

    private void KeySelect()
    {
        if(Input.anyKeyDown && !Input.GetKey(KeyCode.LeftShift))
        {
            for(int num = 1; num <= manager.HotbarSize; num++)
            {
                if(Input.GetKeyDown(num.ToString()))
                {
                    manager.HotbarSelection = num - 1;
                }
            }
        }
    }

    private void ScrollSelect()
    {
        int scrollDelta = 0 - (int)Input.mouseScrollDelta.y;
        int slotCount = manager.HotbarSize;

        if (scrollDelta != 0 && !Input.GetKey(KeyCode.LeftShift))
        {
            manager.HotbarSelection = (manager.HotbarSelection + scrollDelta) % slotCount;

            if (manager.HotbarSelection < 0)
                manager.HotbarSelection += slotCount;
        }
    }

    private void TryUseTool()
    {
        if(Input.GetMouseButtonDown(1))
        {
            manager.Hotbar.TryUseTool(CreateEffectContext());
        }
    }

    private void OnDisable() => manager.OnDisable();
}
