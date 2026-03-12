using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beakon : MonoBehaviour
{
    public float radius;

    private void Update()
    {
        AlertSurroundings();
    }

    private void AlertSurroundings()
    {
        Collider2D[] surroundings = Physics2D.OverlapCircleAll(transform.position, radius);

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
