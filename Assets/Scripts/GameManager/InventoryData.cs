using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/InventoryData")]
public class InventoryData : ScriptableObject
{
    [System.Serializable]
    public class InventoryContainer
    {
        public InventoryEntry[] inventory;

        public InventoryContainer(InventoryEntry[] Inventory)
        {
            inventory = Inventory;
        }
    }


    [SerializeField]
    private InventoryContainer[] inventories;

    [SerializeField]
    private List<HeldItem> itemsHeld;
    [SerializeField]
    private bool isPersisting = false;

    public InventoryEntry[][] Inventories
    {
        get
        {
            InventoryEntry[][] temp = null;

            if(inventories != null)
            {
                temp = new InventoryEntry[inventories.Length][];
                for(int i = 0; i < inventories.Length; i++)
                {
                    temp[i] = inventories[i].inventory;
                }
            }
            
            return temp;
        } 
        set
        {
            inventories = new InventoryContainer[value.Length];
            for(int i = 0; i < value.Length; i++)
            {
                inventories[i] = new InventoryContainer(value[i]);
            }
        }
    }
    public List<HeldItem> ItemsHeld { get => itemsHeld; set => itemsHeld = value; }
    public bool IsPersisting { get => isPersisting; set => isPersisting = value; }
}
