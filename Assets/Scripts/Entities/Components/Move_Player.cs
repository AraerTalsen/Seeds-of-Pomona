using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ClickObjectInWorld))]
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerInteract))]
public class Move_Player : MonoBehaviour
{
    public float moveSpeed, runModifier;
    private bool pauseMovement = false, isHidden = false;

    private Rigidbody2D rb;
    private PlayerInteract playerInteract;
    private ClickObjectInWorld cow;
    public PInv inventory;
    private EntityOrientation entityOrientation;
    Animator animator;
    private Vector2 lastMoveDirection;
    private bool facingLeft = true;
    private Vector2 input;

    public bool IsHidden { get => isHidden; }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInteract = GetComponent<PlayerInteract>();
        cow = GetComponent<ClickObjectInWorld>();
        inventory = GetComponent<PInv>();
        entityOrientation = GetComponent<EntityOrientation>();

        playerInteract.Move_Player = this;

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pauseMovement)
        {
            Move();
        }

        Animate();
        ProcessInputs();
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
    
    public void TogglePauseMovement(bool pause)
    {
        rb.velocity = Vector2.zero;
        pauseMovement = pause;
        cow.PauseWorldClicks = pause;
    }

    public void ToggleHidden()
    {
        isHidden = !isHidden;
    }

    void Animate()
    {
        animator.SetFloat("MoveX", input.x);
        animator.SetFloat("MoveY", input.y);
        animator.SetFloat("MoveMagnitude", input.magnitude);
        animator.SetFloat("LastMoveX", lastMoveDirection.x);
        animator.SetFloat("LastMoveY", lastMoveDirection.y);
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX == 0 && moveY == 0 && (input.x != 0 || input.y != 0))
        {
            lastMoveDirection = input;
            entityOrientation.CurrentOrientation = lastMoveDirection;
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input.Normalize();
    }
}
