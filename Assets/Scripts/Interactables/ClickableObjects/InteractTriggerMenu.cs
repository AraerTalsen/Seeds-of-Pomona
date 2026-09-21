using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicMenu))]
public class InteractTriggerMenu : Interactable
{
    private BasicMenu menu;

    public override void StartInteractiveProcess(GameObject interactor)
    {
        if (menu == null)
        {
            menu = GetComponent<BasicMenu>();
            menu.ToggleMenu(interactor.GetComponent<Move_Player>());
        }
        else if (menu != null) menu.ToggleMenu(interactor.GetComponent<Move_Player>());
    }
}
