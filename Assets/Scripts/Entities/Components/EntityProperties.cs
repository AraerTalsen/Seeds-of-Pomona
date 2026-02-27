using System.Collections.Generic;
using UnityEngine;

public class EntityProperties
{
    public delegate Vector2 ResetTarget();
    public delegate void DelayNextAction(float recoveryTime);

    public DelayNextAction Recover { get; set; }
    public ResetTarget ChoosePatrolPoint { get; set; }

    public float MoveSpeed => StatBlock[Stats.Speed];
    public float TurnSpeed { get; set; }
    public float PatrolRadius { get; set; }
    public float Persistence { get; set; }
    public float HuntRecoveryTime { get; set; }
    
    public bool IsResting { get; set; }
    public bool IsStunned { get; set; }
    public bool IsTracking { get; set; }
    public bool IsTargetLost { get; set; }
    public Transform Transform { get; set; }
    [SerializeField]
    private List<Transform> spottedTargets = new();
    public List<Transform> SpottedTargets => spottedTargets;
    public StatBlock StatBlock { get; set; }

    private Transform targetTransform;
    public Transform TargetTransform
    {
        get => targetTransform;
        set
        {
            if(!IsTargetLost)
            {
                IsTargetLost = targetTransform != null && value == null;
            }
            
            targetTransform = value;
            Vector2? temp = targetTransform != null ? targetTransform.position : null;
            //Do we need all of these casts or can we just do (Vector2)temp or even TargetPos ?= temp;
            TargetPos = temp == null ? temp : new Vector2(((Vector2)temp).x, ((Vector2)temp).y);
        }
    }

    private Vector2? targetPos;
    public Vector2? TargetPos
    {
        get => targetPos;
        set
        {
            targetPos = value ?? ChoosePatrolPoint();
        }
    }

    private Quaternion targetRotation;
    public Quaternion TargetRotation { get => targetRotation; set => targetRotation = value; }

    public Vector2 Rotate()
    {
        Vector2 dirToTarget = (Vector2)TargetPos - (Vector2)Transform.position;
        float angle = Mathf.Atan2(dirToTarget.x, dirToTarget.y) * Mathf.Rad2Deg;
        targetRotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        Transform.rotation = Quaternion.RotateTowards(Transform.rotation, targetRotation, TurnSpeed);

        return dirToTarget;
    }

    public void SpottedNewTarget(Transform t)
    {
        spottedTargets.Add(t);
    }

    public void LostTargets()
    {
        spottedTargets.Clear();
    }
}
