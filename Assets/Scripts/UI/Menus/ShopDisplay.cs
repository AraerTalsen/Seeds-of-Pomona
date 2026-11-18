using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopDisplay
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
    private ShopManager _manager;

    public bool IsGeneEditorEnabled { get; private set; } = false;

    public void Initialize(ShopManager manager, bool isEditorEnabled)
    {
        _manager = manager;
        if(isEditorEnabled)
            EnableGeneEditor();
    }

    public void Purchase(Button clicked)
    {
        string buttonName = clicked.name;
        string titleCardName = clicked.transform.GetChild(0).name;
        string cost = clicked.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text;

        _manager.TryCompleteTransaction(buttonName, titleCardName, cost);
    }

    public void UpdateDisplays()
    {
        int playerBalance = _manager.PlayerInventory.Wallet.CurrentBalance;
        foreach(var ware in wares)
        {
            string cost = ware.Price.text;
            int.TryParse(cost[(cost.IndexOf('$') + 1)..], out int costAsNum);
            ware.OutOfBudgetOverlay.SetActive(costAsNum > playerBalance);

            if(ware.QtyInInv != null)
            {
                int sum = _manager.PlayerInventory.Sum(ware.ID);
                ware.QtyInInv.text = "In Inventory: " + sum.ToString();
            }
        }
    }

    public void EnableGeneEditor()
    {
        geneEditor.SetActive(true);
        soldSign.SetActive(true);
        geneEditorBtn.interactable = false;
        IsGeneEditorEnabled = true;
    }
}
