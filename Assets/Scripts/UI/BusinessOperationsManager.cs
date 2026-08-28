using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BusinessOperationsnManager : BasicMenu
{
    [System.Serializable]
    public class Submenu
    {
        [SerializeField] private GameObject menu;
        [SerializeField] private Button tab;

        public GameObject Menu => menu;
        public Button Tab => tab;
    }
    
    [SerializeField] private JobBoardDisplay jobBoardDisplay;
    [SerializeField] private TextMeshProUGUI playerWallet;
    [SerializeField] private List<Submenu> submenues = new();
    
    public PInv Inv { get; set; }
    public bool isOpen = false;
    
    public UnityEngine.Events.UnityAction CheckPlayerInventory { get; set;}
    public JobBoardDisplay JobBoardDisplay => jobBoardDisplay;

    private int currentTab = 0;

    private void Start()
    {
        GetComponent<JobBoardManager>().InitializeDisplayManager();
        JobBoardDisplay.InitializeVariables();
    }

    private void Update()
    {
        if(isOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            isOpen = false;
        }
    }
    
    public override void ToggleMenu(GameObject menu2 = null, Move_Player mp = null)
    {
        base.ToggleMenu(menu2);

        isOpen = true;

        if (Inv == null)
        {
            Inv = mp.inventory;
        }
        CheckPlayerInventory();
        UpdatePlayerWallet();
        SetSubmenuActivity(currentTab, true);
        if (JobBoardDisplay.JobListings.Count > 0)
        {
            JobBoardDisplay.ActivateJobBoard();
            JobBoardDisplay.LoadJob();
            GetComponent<ShopManager>().PlayerInventory = Inv;
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

    public void UpdatePlayerWallet() => playerWallet.text = "$" + Inv.wallet.CurrentBalance.ToString();

    private void SetSubmenuActivity(int index, bool isActive)
    {
        submenues[index].Menu.SetActive(isActive);
        submenues[index].Tab.interactable = !isActive;
    }
}