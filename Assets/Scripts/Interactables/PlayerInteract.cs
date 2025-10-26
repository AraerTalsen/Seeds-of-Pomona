using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerInteract : InteractController
{
    public Move_Player Move_Player { get; set; }
    public InventoryDisplayManager DisplayManager { get; set; }

    public override void Interact()
    {
        Interactable interact = CurrentPriority;
        if (CurrentPriority != null)
        {
            Move_Player.TogglePauseMovement();

            if (interact is InteractTriggerMenu itm)
            {
                itm.Menu2 = DisplayManager.Menu;
                interact.StartInteractiveProcess(gameObject);
            }
            else
            {
                interact.StartInteractiveProcess(gameObject);
                RemoveInteraction(interact);
                Move_Player.TogglePauseMovement();
            }

        }
    }
    
    public override void AddInteraction(Interactable interact)
    {
        CurrentPriority = interact;
    }

    public override void RemoveInteraction(Interactable interact)
    {
        CurrentPriority = null;
    }

    public override void ClearInteractions()
    {
        CurrentPriority = null;
    }
}
