using UnityEngine;

public class NavigateState : BehaviorState
{
    public override float RecoveryTime { get; } = 4.0f;

    private Vector3 targetPos;
    private Transform trans;

    public override void PerformAction()
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
        targetPos = (Vector3)EntityProps.TargetPos;
        EntityProps.Transform.position = Vector2.MoveTowards(trans.position, targetPos, EntityProps.MoveSpeed * Time.deltaTime);
        EntityProps.Rotate();
    }

    private void HasArrived()
    {
        Debug.Log("Distance: " + Vector2.Distance(EntityProps.Transform.position, targetPos));
        if (Vector2.Distance(EntityProps.Transform.position, targetPos) <= 0.1f)
        {
            ResetContextState();
        }
    }
}
