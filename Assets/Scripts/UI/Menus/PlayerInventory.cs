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
