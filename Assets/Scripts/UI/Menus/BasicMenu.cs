using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private PanelManager pm;
    [SerializeField] private bool isOpenPInv;

    public GameObject ActiveMenu { get => menu; }

    public virtual void ToggleMenu(Move_Player mp = null)
    {
        SignalPanelManager();
    }

    public void SignalPanelManager()
    {
        if(!pm.HasMenu(ActiveMenu))
        {
           pm.AddMenu(ActiveMenu, isOpenPInv);
           pm.ToggleFlow(ActiveMenu);
        }
        else
        {
            pm.ToggleFlow(ActiveMenu);
        }
    }
}
