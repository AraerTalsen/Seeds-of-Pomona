using UnityEngine;

public class ObserveState : BehaviorState
{
    public override float RecoveryTime => 2.0f;

    private Vector2 origin, dirToTarget;

    public override IEffectRuntime CreateEffectRuntime(EffectContext context) => new InstantRuntime(this);

    private class InstantRuntime : IEffectRuntime
    {
        public string EffectName => "Observe";
        public bool IsFinished { get; private set; }

        public InstantRuntime(ObserveState effect)
        {
            effect.Apply();
            IsFinished = true;
        }

        public void Tick() { }
    }

    public void Apply()
    {
        Observe();
        IsLookingAtTarget();
    }

    private void Observe()
    {
        dirToTarget = EntityProps.LookAt();
        origin = EntityProps.Transform.up;
    }

    private void IsLookingAtTarget()
    {
        if (Quaternion.Angle(EntityProps.Face.transform.rotation, EntityProps.TargetRotation) <= 5)
        {
            ResetContextState();
        }
    }
}
