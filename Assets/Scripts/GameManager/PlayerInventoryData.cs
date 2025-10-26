using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlayerInventoryData")]
public class PlayerInventoryData : InventoryData
{
    [SerializeField]
    private int balance;

    public int Balance {get => balance; set => balance = value;}
}
