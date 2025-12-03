using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlayerInventoryData")]
public class PlayerInventoryData : InventoryData
{
    [SerializeField]
    private int balance = 0;
    [SerializeField]
    private bool hasDied = false;

    public int Balance { get => balance; set => balance = value; }
    public bool HasDied { get => hasDied; set => hasDied = value; }
}
