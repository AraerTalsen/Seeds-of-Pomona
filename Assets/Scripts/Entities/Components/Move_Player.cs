using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ClickObjectInWorld))]
[RequireComponent(typeof(PlayerDeathManager))]
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(InventoryDisplayManager))]
[RequireComponent(typeof(PlayerInteract))]
public class Move_Player : MonoBehaviour
{
    public float moveSpeed, runModifier;
    private bool pauseMovement = false, isHidden = false;

    private Rigidbody2D rb;
    private PlayerInteract playerInteract;
    private ClickObjectInWorld cow;
    private PlayerDeathManager pdm;
    public PlayerInventory inventory;

    public bool IsHidden { get => isHidden; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInteract = GetComponent<PlayerInteract>();
        cow = GetComponent<ClickObjectInWorld>();
        pdm = GetComponent<PlayerDeathManager>();
        inventory = GetComponent<PlayerInventory>();

        playerInteract.Move_Player = this;
        playerInteract.DisplayManager = GetComponent<InventoryDisplayManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pauseMovement)
        {
            Move();
        }
    }

    private void Move()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float h = Input.GetAxisRaw("Horizontal"), v = Input.GetAxisRaw("Vertical");
        Vector2 moveVector = new Vector2(h, v).normalized * (moveSpeed + (isRunning ? runModifier : 0));
        rb.velocity = moveVector;
    }

    public void TogglePauseMovement()
    {
        rb.velocity = Vector2.zero;
        pauseMovement = !pauseMovement;
        cow.PauseWorldClicks = !cow.PauseWorldClicks;
    }

    public void ToggleHidden()
    {
        isHidden = !isHidden;
    }

    public void Kill()
    {
        pdm.KillPlayer();
    }
}
