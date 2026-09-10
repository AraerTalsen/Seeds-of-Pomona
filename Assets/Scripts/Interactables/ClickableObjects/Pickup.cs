using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactable
{
    public int itemId;
    
    public override void StartInteractiveProcess(GameObject interactor)
    {
        PlayerInventory inv = interactor.GetComponent<Move_Player>().inventory;
        inv.PushItems(itemId, 1, out _);
        TrySpawnSpecial(inv);
        Destroy(transform.parent.gameObject);
    }

    private void TrySpawnSpecial(PlayerInventory inv)
    {
        int specialId = ItemDictionary.items[itemId].SelectSpecialItem();
        if(specialId > -1)
        {
            bool isUniqueInstance = ItemDictionary.items[specialId] is PowerUp;
            inv.PushItems(specialId, 1, out _, isUniqueInstance);
        }
    }
}
