using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDeathManager : MonoBehaviour
{
    [SerializeField] protected EntityStats stats;
    protected bool isDying = false;
    
    private void Update()
    {
        if(!isDying)
        {
            KillEntity();
        }
    }
    
    protected virtual void KillEntity()
    {
        if(stats.CurrentHealth <= 0)
        {
            isDying = true;
            Destroy(gameObject);
        }       
    }
}
