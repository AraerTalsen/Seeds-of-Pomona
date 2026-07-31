using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/States/Observe State")]
public class ObserveState : BehaviorStateRuntime
{
    public override float RecoveryTime => 2.0f;

    private Vector2 origin, dirToTarget;

    private void Observe()
    {
        try
        {
            if(EntityProps.NavMeshAgent.isActiveAndEnabled || EntityProps.IsHalted)
            {
                dirToTarget = EntityProps.LookAt();
                origin = EntityProps.Transform.up;

                //if(!EntityProps.IsHalted) await EntityProps.ToggleEntityHalt(true);
                EntityProps.NavMeshAgent.isStopped = true;
                EntityProps.Animator.SetFloat("moveMagnitude", 0);
                
            }
            //else Debug.Log("Agent is not active and enabled");
        }
        catch(Exception e)
        {
            Debug.LogError($"Failed to toggle enemy's halt: {e}");
        }
    }

    private void IsLookingAtTarget()
    {
        bool isLookingAt = Quaternion.Angle(EntityProps.Face.transform.rotation, EntityProps.TargetRotation) <= 0.1;
        if(isLookingAt) 
        {
            if(EntityProps.IsTargetLost && !EntityProps.IsTracking)
            {
                EntityStateSupport.QuitSearch();
            }
            Recover();
        }
    }

    public override void TickProcess(EffectContext context) 
    {
        Observe();
        IsLookingAtTarget();
    }
}
