using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/InventoryData")]
public class InventoryData : PersistentDataBase
{
    [SerializeField] private List<InventoryEntry> inventory;

    [SerializeField]
    private bool isPersisting = false;
    public List<InventoryEntry> Inventory { get => inventory; set => inventory = value; }
    public bool IsPersisting { get => isPersisting; set => isPersisting = value; }

    public void ClearInventory()
    {
        for(int i = 0; i < Inventory.Count; i++)
        {
            Item item = Inventory[i].Item;
            Inventory[i].Remove();
            if(item is PUp) UniqueItemManager.Delete(item);
        }
    }
}
