using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Interactable : MonoBehaviour, IClickable
{
    [SerializeField]
    private float range = 1;
    [SerializeField]
    private bool isHighlightable = true;

    private (float x, float y) spriteRange;
    private SpriteRenderer sr;
    protected bool IsInteractable { get; set; }= true;
    [SerializeField] private Sprite unselected;
    [SerializeField] private Sprite highlight;
    [SerializeField] private Sprite highlightOutOfRange;

    public float Range { get => range; }

    private void Start()
    {
        if (isHighlightable)
        {
            sr = GetComponent<SpriteRenderer>();
            spriteRange.x = sr.sprite.rect.size.x / sr.sprite.pixelsPerUnit / 2;
            spriteRange.y = sr.sprite.rect.size.y / sr.sprite.pixelsPerUnit / 2;
        }
        else
        {
            spriteRange = (0, 0);
        }
    }

    public abstract void StartInteractiveProcess(GameObject interactor);

    public void OnHoverEnter(GameObject player)
    {
        //print($"Name: {transform} Range: {spriteRange} Sprite Size: {sr.sprite.rect.size} Player Distance: {Vector2.Distance(player.transform.position, transform.position)}");
        if (IsInteractable)
        {
            player.GetComponent<PlayerInteract>().AddInteraction(this);
            UpdateVisualCues(player);
        }
    }

    public void OnHover(GameObject player)
    {
        if (IsInteractable)
        {
            UpdateVisualCues(player);
        }
    }

    public void OnHoverExit(GameObject player)
    {
        if (IsInteractable)
        {
            player.GetComponent<PlayerInteract>().RemoveInteraction(this);
            UpdateHighlightVisual(unselected);
            CursorManager.CurrentCursor = CursorManager.CursorType.DEFAULT;
        }
    }

    public void OnClick(GameObject player)
    {
        if (IsInteractable && IsInRange(player))
        {
            OnHoverExit(player);
            StartInteractiveProcess(player);
        }
    }

    private void UpdateVisualCues(GameObject player)
    {
        UpdateHighlightVisual(IsInRange(player) ? highlight : highlightOutOfRange);
        CursorManager.CurrentCursor = IsInRange(player) ? CursorManager.CursorType.INTERACT : CursorManager.CursorType.INTERACT_GHOST;
    }

    private void UpdateHighlightVisual(Sprite sprite)
    {
        if (isHighlightable)
        {
            sr.sprite = sprite;
        }
    }

    private bool IsInRange(GameObject interactor)
    {
        float distFromEntity = Vector2.Distance(transform.position, interactor.transform.position);
        bool inXRange = Range >= distFromEntity - spriteRange.x;
        bool inYRange = Range >= distFromEntity - spriteRange.y;
        return inXRange && inYRange;
    }
}
