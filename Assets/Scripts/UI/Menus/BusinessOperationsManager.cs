using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BusinessOperationsManager : BasicMenu
{
    [System.Serializable]
    public class Submenu
    {
        [SerializeField] private GameObject menu;
        [SerializeField] private Button tab;

        public GameObject Menu => menu;
        public Button Tab => tab;
    }
    
    [SerializeField] private List<Submenu> submenues = new();
    [SerializeField] private TextMeshProUGUI playerWallet;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private JobBoardManager jobBoardManager;
    private JobBoardDisplay jobBoardDisplay;
    
    public PlayerInventory Inv { get; set; }
    public bool isOpen = false;
    
    public UnityEngine.Events.UnityAction CheckPlayerInventory { get; set;}

    private int currentTab = 0;

    private void Start()
    {
        shopManager.Initialize(this);
        jobBoardManager.Initialize(shopManager, this);
        jobBoardDisplay = jobBoardManager.JobBoardDisplay;
        jobBoardDisplay.Initialize();
    }

    private void FixedUpdate()
    {
        jobBoardManager.ClearOldJobs();
    }

    public override void ToggleMenu(Move_Player mp)
    {
        base.ToggleMenu();

        isOpen = true;

        if (Inv == null)
        {
            Inv = mp.inventory;
        }
        CheckPlayerInventory();
        UpdatePlayerWallet();
        SetSubmenuActivity(currentTab, true);
        if (jobBoardDisplay.JobListings.Count > 0)
        {
            jobBoardDisplay.ActivateJobBoard();
            jobBoardDisplay.LoadJob();
            shopManager.PlayerInventory = Inv;
            shopManager.UpdateDisplays();
        }
    }

    private void Update()
    {
        if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) && isOpen)
        {
            isOpen = false;
        }
    }

    public void SetSubmenuToCurrent(Button clicked)
    {
        int index = submenues.FindIndex( s => s.Tab == clicked );

        if(index > -1)
        {
            SetSubmenuActivity(currentTab, false);
            SetSubmenuActivity(index, true);

            currentTab = index;
        }
    }

    public void Purchase(Button button) => shopManager.Purchase(button);

    public void UpdatePlayerWallet() => playerWallet.text = "$" + Inv.Wallet.CurrentBalance.ToString();

    private void SetSubmenuActivity(int index, bool isActive)
    {
        submenues[index].Menu.SetActive(isActive);
        submenues[index].Tab.interactable = !isActive;
    }

    private void OnDisable() => jobBoardManager.OnDisable();
}