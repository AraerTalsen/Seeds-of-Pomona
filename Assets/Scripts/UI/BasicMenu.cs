using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private PanelManager pm;
    [SerializeField] private bool openPlayerInventory = false;

    public GameObject Menu {get => menu; set => menu = value; }
    public GameObject ActiveMenu { get => menu; }

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
