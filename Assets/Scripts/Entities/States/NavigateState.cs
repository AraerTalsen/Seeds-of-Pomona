using UnityEngine;

public class NavigateState : BehaviorState
{
    public override float RecoveryTime { get; } = 4.0f;

    private Vector2 targetPos;
    private Transform trans;

    public override IEffectRuntime CreateEffectRuntime(EffectContext context) => new InstantRuntime(this);

    private class InstantRuntime : IEffectRuntime
    {
        public string EffectName => "Navigate";
        public bool IsFinished { get; private set; }

        public InstantRuntime(NavigateState effect)
        {
            effect.Apply();
            IsFinished = true;
        }

        public void Tick() { }
    }

    public void Apply()
    {
        if (trans == null)
        {
            trans = EntityProps.Transform;
        }
        Move();
        HasArrived();
    }

    private void Move()
    {
        targetPos = (Vector2)EntityProps.TargetPos;
        Vector2 moveDir = (targetPos - (Vector2)trans.position).normalized;
        EntityProps.NavMeshAgent.velocity = moveDir * EntityProps.MoveSpeed;
        EntityProps.LookAt();
    }

    private void HasArrived()
    {
        if (Vector2.Distance(EntityProps.Transform.position, targetPos) <= 0.1f)
        {
            EntityProps.NavMeshAgent.velocity = Vector2.zero;

            Vector2? susSpot = EntityProps.SuspiciousSpot;
            if(susSpot != null && Vector2.Distance((Vector2)EntityProps.TargetPos, (Vector2)susSpot) <= 0.1f)
            {
                EntityProps.SuspiciousSpot = null;
            }
            
            if(EntityProps.IsTargetLost && !EntityProps.IsTracking)
            {
                EntityStateSupport.QuitSearch();
            }
            ResetContextState();
        }
    }
}
