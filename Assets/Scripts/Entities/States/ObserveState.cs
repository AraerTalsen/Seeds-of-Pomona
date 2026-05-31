using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/States/Observe State")]
public class ObserveState : BehaviorStateRuntime
{
    public override float RecoveryTime => 2.0f;

    private Vector2 origin, dirToTarget;

    private void Observe()
    {
        if(EntityProps.NavMeshAgent.isActiveAndEnabled)
        {
            dirToTarget = EntityProps.LookAt();
            origin = EntityProps.Transform.up;
            EntityProps.NavMeshAgent.isStopped = true;
        }
        else Debug.Log("Agent is not active and enabled");
    }

    private bool IsLookingAtTarget()
    {
        return Quaternion.Angle(EntityProps.Face.transform.rotation, EntityProps.TargetRotation) <= 5;
    }

    public override void TickProcess(EffectContext context) => Observe();

    public override bool EndCondition(EffectContext context) => IsLookingAtTarget();
}
