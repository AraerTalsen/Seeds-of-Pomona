using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move_Player))]
public class PlayerSleepManager : MonoBehaviour
{
    [SerializeField]
    private Animator screenFX;
    private Move_Player move_Player;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        move_Player = GetComponent<Move_Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void Sleep()
    {
        screenFX.SetTrigger("isSleeping");
        move_Player.TogglePauseMovement();
        spriteRenderer.sortingOrder = 2;
    }

    public void WakeUp()
    {
        move_Player.TogglePauseMovement();
        transform.position += Vector3.right * 2;
        spriteRenderer.sortingOrder = 1;
    }
}
