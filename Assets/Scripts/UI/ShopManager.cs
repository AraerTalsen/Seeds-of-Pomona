using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopWare
    {
        [SerializeField] private int id;
        [SerializeField] private TextMeshProUGUI qtyInInv;
        [SerializeField] private TextMeshProUGUI price;
        [SerializeField] private GameObject outOfBoundsOverlay;

        public int ID => id;
        public TextMeshProUGUI QtyInInv => qtyInInv;
        public TextMeshProUGUI Price => price;
        public GameObject OutOfBudgetOverlay => outOfBoundsOverlay;
    }

    [SerializeField] private GameObject geneEditor;
    [SerializeField] private Button geneEditorBtn;
    [SerializeField] private GameObject soldSign;
    [SerializeField] private List<ShopWare> wares = new();

    private PInv playerInventory;
    public PInv PlayerInventory 
    { 
        get => playerInventory;
        set
        {
            playerInventory = value;
            UpdateDisplays();
        }
    }
    public bool IsGeneEditorEnabled { get; private set; } = false;
    private BusinessOperationsnManager busOp;

    private void Start()
    {
        busOp = GetComponent<BusinessOperationsnManager>();
    }

    public void Initialize() => EnableGeneEditor();
    
    public void Purchase(Button clicked)
    {
        string buttonName = clicked.name;
        string titleCardName = clicked.transform.GetChild(0).name;
        string cost = clicked.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text;

        TryCompleteTransaction(buttonName, titleCardName, cost);
    }

    private void TryCompleteTransaction(string buttonName, string titleCardName, string cost)
    {
        if(int.TryParse(cost[(cost.IndexOf('$') + 1)..], out int costAsNum) && PlayerInventory.wallet.CurrentBalance >= costAsNum)
        {
            PlayerInventory.wallet.DecrementBalance(costAsNum);
            busOp.UpdatePlayerWallet();
            PurchaseWares(buttonName, titleCardName);
        }
    }

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
                EnableGeneEditor();
            }
            UpdateDisplays();
        }
    }

    public void UpdateDisplays()
    {
        int playerBalance = PlayerInventory.wallet.CurrentBalance;
        foreach(var ware in wares)
        {
            string cost = ware.Price.text;
            int.TryParse(cost[(cost.IndexOf('$') + 1)..], out int costAsNum);
            ware.OutOfBudgetOverlay.SetActive(costAsNum > playerBalance);

            if(ware.QtyInInv != null)
            {
                int sum = PlayerInventory.Sum(ware.ID);
                ware.QtyInInv.text = "In Inventory: " + sum.ToString();
            }
        }
    }

    private void EnableGeneEditor()
    {
        geneEditor.SetActive(true);
        soldSign.SetActive(true);
        geneEditorBtn.interactable = false;
        IsGeneEditorEnabled = true;
    }
}
