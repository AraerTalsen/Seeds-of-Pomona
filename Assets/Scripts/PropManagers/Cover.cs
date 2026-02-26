using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Cover : MonoBehaviour
{
    [SerializeField]
    private Color coverColor;
    private Color defaultColor = Color.white;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Move_Player mp = other.gameObject.GetComponent<Move_Player>();
            mp.ToggleHidden();
            spriteRenderer.color = coverColor;
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Move_Player mp = other.gameObject.GetComponent<Move_Player>();
            mp.ToggleHidden();
            spriteRenderer.color = defaultColor;
        }
    }
}
