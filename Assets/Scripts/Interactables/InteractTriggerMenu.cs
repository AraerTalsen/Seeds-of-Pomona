using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicMenu))]
public class InteractTriggerMenu : Interactable
{
    private BasicMenu menu;
    //We might not want to have menu2 in all instances of InteractTriggerMenu. What's a better approach?
    private GameObject menu2;

    public GameObject Menu2
    {
        get
        {
            return menu2;
        }
        set
        {
            menu2 = value;
        }
    }

    public override void StartInteractiveProcess(GameObject interactor)
    {
        interactor.GetComponent<Move_Player>().TogglePauseMovement();
        if (menu == null && GetComponent<BasicMenu>() != null)
        {
            menu = GetComponent<BasicMenu>();
            menu.ToggleMenu(menu2, interactor.GetComponent<Move_Player>());
        }
        else if (menu != null) menu.ToggleMenu(menu2);
    }
}
