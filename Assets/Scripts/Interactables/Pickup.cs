using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactable
{
    public int itemId;
    
    public override void StartInteractiveProcess(GameObject interactor)
    {
        interactor.GetComponent<Move_Player>().inventory.GetInventory().PushItems(1, 1);
        Destroy(transform.parent.gameObject);
    }
}
