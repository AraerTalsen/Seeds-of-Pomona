using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/States/Navigate State")]
public class NavigateState : BehaviorStateRuntime
{
    public override float RecoveryTime { get; } = 4.0f;

    private void Move()
    {
        if(EntityProps.NavMeshAgent.isActiveAndEnabled)
        {
            EntityProps.NavMeshAgent.isStopped = false;
            EntityProps.LookAt();
        }
        else Debug.Log("Agent is not active and enabled");
    }

    private bool HasArrived()
    {
        bool inRange = EntityProps.DistFromTargetPos <= 0.1f;
        if (inRange)
        {
            EntityProps.NavMeshAgent.isStopped = true;

            Vector2? susSpot = EntityProps.SuspiciousSpot;
            if(susSpot != null && Vector2.Distance((Vector2)EntityProps.TargetPos, (Vector2)susSpot) <= 0.1f)
            {
                EntityProps.SuspiciousSpot = null;
            }
            
            if(EntityProps.IsTargetLost && !EntityProps.IsTracking)
            {
                EntityStateSupport.QuitSearch();
            }
        }

        return inRange;
    }

    public override void TickProcess(EffectContext context) => Move();

    public override bool EndCondition(EffectContext context) => HasArrived();
}
