using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private int damage; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(rb.velocity.magnitude < 4)
            DestroyProjectile();
    }

    public void FireProjectile(Vector2 force, int dmg)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
        damage = dmg;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out EntityStats stats))
        {
            stats.CurrentHealth -= damage;

            EntityStateSupport support = collision.gameObject.GetComponent<EntityStateSupport>();
            collision.gameObject.GetComponent<EntityManager>().EntityProps.TargetTransform = null;
            support.Stun(3);
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
