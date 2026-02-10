using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

public class PInv : PersistentObject<PlayerInventoryData>
{    
    public PlayerInventoryData persist;
    public int maxCapacity;
    public Wallet wallet;
    [SerializeField] private Transform invContainer;
    private DragNDropInventory bag;

    private void Start()
    {
        Persist = RetrieveData(persist);
        PullData();
    }

    protected override void PullData()
    {
        bag = new(maxCapacity, invContainer);
        if (!Persist.IsPersisting)
        {
            Persist.Inventory = bag.Entries.ToList();
            Persist.IsPersisting = true;
        }
        else
        {
            bag.LoadFromStorage(Persist.Inventory);
            wallet.CurrentBalance = Persist.Balance;
        }
    }

    protected override void PushData()
    {
        Persist.IsPersisting = true;
        Persist.Balance = wallet.CurrentBalance;
    }

    public void PushDataTemp()
    {
        PushData();
    }

    public DragNDropInventory GetInventory() => bag;

    public void TriggerDeath()
    {
        Persist.HasDied = true;
    }
}
