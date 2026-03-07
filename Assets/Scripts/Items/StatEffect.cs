using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Stat Effect")]
public class StatEffect : ContactEffect
{
    [SerializeField] private int modifier;

    protected override void Apply(EffectContext context)
    {
        Rigidbody2D rb = context.targetBody.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        base.Apply(context);
    }

    protected override bool Callback(Collider2D other)
    {
        if(other.TryGetComponent(out EntityStats entityStats))
        {
            entityStats.CurrentHealth = modifier;
            return true;
        }
        return false;
    }
}
