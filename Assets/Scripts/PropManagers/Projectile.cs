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
        Debug.Log($"Instance {GetInstanceID()} is awake and has Rigidbody2D: {rb != null}");
    }

    private void Update()
    {
        if(rb.velocity.magnitude < 4)
        {
            Debug.Log("Projectile missed");
            DestroyProjectile();
        }
    }

    public void FireProjectile(Vector2 force, int dmg)
    {
        Debug.Log($"Instance {GetInstanceID()} is firing and has a Rigidbody2D: {rb != null}");
        rb.AddForce(force, ForceMode2D.Impulse);
        damage = dmg;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collided with {collision.gameObject}");
        if(collision.gameObject.TryGetComponent(out EntityStats stats))
        {
            stats.CurrentHealth -= damage;

            if(collision.gameObject.TryGetComponent(out EntityStateSupport support))
            {
                collision.gameObject.GetComponent<EntityManager>().EntityProps.TargetTransform = null;
                support.Stun(3);
                DestroyProjectile();
            }
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
