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
        //Vector2 moveDir = ((Vector2)EntityProps.LookAtPoint.position - (Vector2)trans.position).normalized;
        //EntityProps.NavMeshAgent.velocity = moveDir * EntityProps.MoveSpeed;
        EntityProps.NavMeshAgent.isStopped = false;
        EntityProps.LookAt();
    }

    private void HasArrived()
    {
        //Debug.Log($"Target Pos: {EntityProps.TargetPos}, Current Pos: {EntityProps.Transform.position}, Distance: {}")
        if (EntityProps.DistFromTargetPos <= 0.1f)
        {
            //EntityProps.NavMeshAgent.velocity = Vector2.zero;
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
            ResetContextState();
        }
    }
}
