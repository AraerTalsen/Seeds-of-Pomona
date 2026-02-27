using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PInv : PersistentObject<PlayerInventoryData>
{    
    [SerializeField] private FlexInvDisplayManager.ISlotPrefill prefill;
    //[SerializeField] private BoonDisplay.BoonDisplayProps props;
    [SerializeField] private PowerupContext powerupContext;
    [SerializeField] private Transform bagContainer, powerupContainer;
    [SerializeField][TextArea] private string deathMsg;
    [SerializeField] private GameObject HUDSlot;
    [SerializeField] private Transform HUDContainer;
    public PlayerInventoryData persist;
    public int bagCapacity, powerupCapacity;
    public Wallet wallet;
    private BoundedDDI bag;
    private PowerUps powerupSlots;
    //private BoonProfile boonProfile;
    private EntityStats stats;
    private PowerupHelper powerupHelper;

    private void Start()
    {
        stats = GetComponent<EntityStats>();
        powerupHelper = GetComponent<PowerupHelper>();
        powerupContext = new()
        {
          targetBody = transform,
          stats = stats.Stats,
          orientation = GetComponent<EntityOrientation>()
        };
        powerupHelper.Context = powerupContext;
        Persist = RetrieveData(persist);
        PullData();
        bag.PushItems(3, 1);
        bag.PushItems(4, 1);
    }

    private void Update()
    {
        powerupSlots.PowerupInterface();
    }

    protected override void PullData()
    {
        bag = new(bagContainer);
        powerupSlots = new(gameObject, powerupCapacity, powerupContainer, prefill, HUDSlot, HUDContainer, powerupHelper);
        //boonProfile = new(props, stats.Stats);

        if (!Persist.IsPersisting)
        {
            Persist.Inventory = bag.Entries;
            Persist.Powerups = powerupSlots.Entries;
            //Persist.Boons = boonProfile.Modifiers;
            Persist.IsPersisting = true;
        }
        else
        {
            bag.LoadFromStorage(Persist.Inventory);
            powerupSlots.RebuildSlots(Persist.Powerups, Persist.LockStates);
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
        
        Persist.IsPersisting = true;
        Persist.Balance = wallet.CurrentBalance;

        List<bool> lockStates = new();
        for(int i = 0; i < powerupSlots.Count; i++)
        {
            lockStates.Add(powerupSlots.IsSlotLocked(i));
        }
        Persist.LockStates = lockStates;
    }

    public void PushDataTemp()
    {
        PushData();
    }

    public BoundedDDI GetInventory() => bag;
    public PowerUps GetPowerups() => powerupSlots;

    public void TriggerDeath()
    {
        Persist.HasDied = true;
    }

    private void LogDeathMessage()
    {
        Persist.HasDied = false;
        TextWindowManager.Instance.SetMessage(deathMsg, gameObject);
    }

    private void OnDisable()
    {
        
    }
}
