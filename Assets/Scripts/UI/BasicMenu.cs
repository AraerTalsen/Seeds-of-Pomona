using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private bool createMenuInstance = true;
    public PanelManager pm;
    public bool openPlayerInventory = false;

    public GameObject Menu {get => menu;}
    public GameObject ActiveMenu { get; private set; }

    private void Awake()
    {
        if (createMenuInstance)
        {
            SetActiveMenu();
        }
        else
        {
            ActiveMenu = menu;
        }
    }

    protected void SetActiveMenu()
    {
        if (ActiveMenu == null)
        {
            ActiveMenu = Instantiate(menu);
        }
    }

    //Is there a better way to access the player inventory data structure than this since only the Job Board needs it right now?
    public virtual void ToggleMenu(GameObject menu2 = null, Move_Player mp = null)
    {
        SignalPanelManager();
    }

    public void SignalPanelManager()
    {
        if(!pm.HasMenu(ActiveMenu))
        {
           pm.AddMenu(ActiveMenu, openPlayerInventory);
           pm.ToggleMenus(ActiveMenu);
        }
        else
        {
            pm.ToggleMenus(ActiveMenu);
        }
    }
}
