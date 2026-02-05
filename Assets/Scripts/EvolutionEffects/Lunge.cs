using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Lunge")]
public class Lunge : AbilityState
{
    [SerializeField]
    private float lungeDist;
    [SerializeField]
    private float lungeSpeed;
    private EntityStateSupport.BehaviorType Behavior;

    public override EntityStateSupport EntityStateSupport
    {
        get => entityStateSupport;
        set
        {
            entityStateSupport = value;
            Behavior = LungeForward;
        }
    }
    private EntityStateSupport entityStateSupport;


    public override void PerformAction()
    {
        EntityStateSupport.LaunchBehavior(Behavior, lungeSpeed);
    }

    private void LungeForward()
    {
        Debug.Log("Lunged");
    }
}
