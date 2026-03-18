using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    public float radius;
    private CircleCollider2D col;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
        col.radius = radius;
    }

    private void Update()
    {
        AlertSurroundings();
    }

    private void AlertSurroundings()
    {
        Collider2D[] surroundings = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Hurtbox"));

        foreach(Collider2D other in surroundings)
        {
            if(other.gameObject.TryGetComponent(out EntityManager manager))
            {
                EntityProperties props = manager.EntityProps;
                if(props.TargetTransform == null)
                {
                    props.SuspiciousSpot = transform.position;
                }
            }
        }
    }
}
