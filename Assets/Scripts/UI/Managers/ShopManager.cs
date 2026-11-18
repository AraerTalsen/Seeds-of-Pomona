using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopManager
{
    [SerializeField] private ShopDisplay shopDisplay;
    private PlayerInventory playerInventory;
    private BusinessOperationsManager busOp;

    public PlayerInventory PlayerInventory 
    { 
        get => playerInventory;
        set
        {
            playerInventory = value;
        }
    }
    public bool IsGeneEditorEnabled => shopDisplay.IsGeneEditorEnabled;

    public void Initialize(BusinessOperationsManager manager)
    {
        busOp = manager;
    }

    public void InitializeDisplay(bool isEditorEnabled) => shopDisplay.Initialize(this, isEditorEnabled);   

    public void UpdateDisplays() => shopDisplay.UpdateDisplays();

    public void TryCompleteTransaction(string buttonName, string titleCardName, string cost)
    {
        if(int.TryParse(cost[(cost.IndexOf('$') + 1)..], out int costAsNum) && PlayerInventory.Wallet.CurrentBalance >= costAsNum)
        {
            PlayerInventory.Wallet.DecrementBalance(costAsNum);
            busOp.UpdatePlayerWallet();
            PurchaseWares(buttonName, titleCardName);
        }
    }

    public void Purchase(Button button) => shopDisplay.Purchase(button);

    private void PurchaseWares(string buttonName, string titleCardName)
    {
        if(int.TryParse(buttonName, out int objType))
        {
            if(objType == 0)
            {
                if(int.TryParse(titleCardName, out int itemId))
                {
                    PlayerInventory.PushItems(itemId, 1, out _);
                }
            }
            else
            {
                shopDisplay.EnableGeneEditor();
            }
            shopDisplay.UpdateDisplays();
        }
    }
}
