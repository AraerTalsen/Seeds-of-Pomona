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
    public SpriteRenderer sr;
    protected bool isInteractable = true;
    private Color highlight = new(255, 235, 0, 255);
    private Color hilightOutOfRange = new(255, 255, 255, 255);
    private Color unselected = new(255, 255, 255, 0);

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

    public void ToggleInteractability()
    {
        isInteractable = !isInteractable;
    }

    public void OnHoverEnter(GameObject player)
    {
        //print($"Name: {transform} Range: {spriteRange} Sprite Size: {sr.sprite.rect.size} Player Distance: {Vector2.Distance(player.transform.position, transform.position)}");
        if (isInteractable)
        {
            player.GetComponent<PlayerInteract>().AddInteraction(this);
            UpdateVisualCues(player);
        }
    }

    public void OnHover(GameObject player)
    {
        if (isInteractable)
        {
            UpdateVisualCues(player);
        }
    }

    public void OnHoverExit(GameObject player)
    {
        if (isInteractable)
        {
            player.GetComponent<PlayerInteract>().RemoveInteraction(this);
            UpdateHighlightColor(unselected);
            CursorManager.CurrentCursor = CursorManager.CursorType.DEFAULT;
        }
    }

    public void OnClick(GameObject player)
    {
        if (isInteractable && IsInRange(player))
        {
            OnHoverExit(player);
            StartInteractiveProcess(player);
        }
    }

    private void UpdateVisualCues(GameObject player)
    {
        UpdateHighlightColor(IsInRange(player) ? highlight : hilightOutOfRange);
        CursorManager.CurrentCursor = IsInRange(player) ? CursorManager.CursorType.INTERACT : CursorManager.CursorType.INTERACT_GHOST;
    }

    private void UpdateHighlightColor(Color c)
    {
        if (isHighlightable)
        {
            sr.color = c;
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
