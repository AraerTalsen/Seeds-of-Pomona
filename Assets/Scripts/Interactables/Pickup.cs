using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactable
{
    public int itemId;
    
    public override void StartInteractiveProcess(GameObject interactor)
    {
        BoundedDDI inv = interactor.GetComponent<Move_Player>().inventory.GetInventory();
        inv.PushItems(itemId, 1);
        TrySpawnSpecial(inv);
        Destroy(transform.parent.gameObject);
    }

    private void TrySpawnSpecial(BoundedDDI inv)
    {
        int specialId = ItemDictionary.items[itemId].SelectSpecialItem();
        if(specialId > -1)
        {
            inv.PushItems(specialId, 1);
        }
    }
}
