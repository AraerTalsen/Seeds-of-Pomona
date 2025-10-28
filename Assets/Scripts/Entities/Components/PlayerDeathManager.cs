using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(Move_Player))]
public class PlayerDeathManager : MonoBehaviour
{
    [SerializeField]
    private Animator screenFX;

    private PlayerInventory inventory;
    private Move_Player mp;

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        mp = GetComponent<Move_Player>();
    }

    public void KillPlayer()
    {
        screenFX.SetTrigger("hasDied");
        mp.TogglePauseMovement();
        inventory.ClearInventory();
        inventory.PushData();
    }
}
