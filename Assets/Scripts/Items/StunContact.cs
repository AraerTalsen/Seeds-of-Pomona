using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Stun Effect")]
public class StunContact : ContactEffect
{
    [SerializeField] private float duration;
    protected override bool Callback(Collider2D other)
    {
        if(other.gameObject.TryGetComponent<EntityManager>(out var manager))
        {
            EntityStateSupport support = other.gameObject.GetComponent<EntityStateSupport>();
            manager.EntityProps.TargetTransform = null;
            support.Stun(duration);
            return true;
        }
        return false;
    }
}
