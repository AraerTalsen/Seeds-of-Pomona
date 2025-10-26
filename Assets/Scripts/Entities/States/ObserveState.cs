using UnityEngine;

public class ObserveState : BehaviorState
{
    public override float RecoveryTime { get; } = 2.0f;

    private Vector2 origin, dirToTarget;

    public override void PerformAction()
    {
        Observe();
        IsLookingAtTarget();
    }

    private void Observe()
    {
        dirToTarget = EntityProps.Rotate();
        origin = EntityProps.Transform.up;
    }

    private void IsLookingAtTarget()
    {
        if (Quaternion.Angle(EntityProps.Transform.rotation, EntityProps.TargetRotation) <= 5)
        {
            ResetContextState();
        }
    }
}
