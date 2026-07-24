using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/States/Navigate State")]
public class NavigateState : BehaviorStateRuntime
{
    public override float RecoveryTime { get; } = 4.0f;

    private void Move()
    {
        try
        {
            if(EntityProps.NavMeshAgent.isActiveAndEnabled || EntityProps.IsHalted)
            {
                
                //if(EntityProps.IsHalted) await EntityProps.ToggleEntityHalt(false);
                EntityProps.NavMeshAgent.isStopped = false;
                EntityProps.UpdateDestination();
                EntityProps.LookAt();
                
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"Failed to toggle enemy's halt: {e}");
        }
    }

    private void HasArrived()
    {
        bool inRange = EntityProps.DistFromTargetPos <= 0.1f;
        
        if(inRange)
        {
            Vector2? susSpot = EntityProps.SuspiciousSpot;
            if(susSpot != null && Vector2.Distance((Vector2)EntityProps.TargetPos, (Vector2)susSpot) <= 0.1f)
            {
                EntityProps.SuspiciousSpot = null;
            }
            
            if(EntityProps.IsTargetLost && !EntityProps.IsTracking && !EntityProps.IsRetreating)
            {
                EntityStateSupport.QuitSearch();
            }
            Recover();
        }
    }

    public override void TickProcess(EffectContext context) 
    {
        Move();
        HasArrived();
        Vector2 targetPos = (Vector2)EntityProps.TargetPos;
        Vector2 steeringPos = EntityProps.NavMeshAgent.steeringTarget;
        bool isInFocus = EntityProps.TargetTransform == null || EntityProps.TargetTransform != null && EntityProps.IsTargetInFocus();
        //Debug.Log($"Is target in focus: {isInFocus}. Has Target: {EntityProps.TargetTransform != null}, Target in focus angle: {EntityProps.TargetTransform != null && EntityProps.IsTargetInFocus()}");
        //Debug.Log($"Navigating to TargetPos: {targetPos}. Looking towards: {(isInFocus ? steeringPos : targetPos)}");

    }
}
