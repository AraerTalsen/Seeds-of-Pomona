using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Stat Effect")]
public class StatEffect : ContactEffect
{
    [SerializeField] private int modifier;

    protected override bool Callback(Collider2D other)
    {
        Debug.Log("Running callback");
        if(other.TryGetComponent(out EntityStats entityStats))
        {
            Debug.Log($"Bashing {other.name}");
            entityStats.CurrentHealth -= modifier;
            return true;
        }
        return false;
    }
}
