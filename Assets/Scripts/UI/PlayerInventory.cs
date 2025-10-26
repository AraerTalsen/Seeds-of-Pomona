using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory
{
    public Wallet wallet;
    public PlayerInventoryData overridePersist;

    protected override void Start()
    {
        InitializeInvStorage();
        persist = RetrievePersistentData();
        overridePersist = (PlayerInventoryData)persist;
        InitializeSaveData(overridePersist);
    }

    public void PushData()
    {
        overridePersist.IsPersisting = true;
        overridePersist.Balance = wallet.CurrentBalance;
    }

    public override void PullData()
    {
        wallet.CurrentBalance = overridePersist.Balance;
    }
}
