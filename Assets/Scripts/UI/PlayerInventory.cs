using UnityEngine;

public class PlayerInventory : Inventory
{
    [SerializeField]
    private string deathMsg;
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
        if (overridePersist.HasDied)
        {
            LogDeathMessage();
        }
    }
    
    public void TriggerDeath()
    {
        overridePersist.HasDied = true;
    }

    private void LogDeathMessage()
    {
        overridePersist.HasDied = false;
        TextWindowManager.Instance.SetMessage(deathMsg, gameObject);
    }
}
