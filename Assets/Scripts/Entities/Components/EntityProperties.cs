using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EntityProperties
{
    public delegate Vector2 ResetTarget();
    public delegate void DelayNextAction(float recoveryTime);
    public delegate void DelayNextMove(IBehaviorState state, float recoveryTime);

    public DelayNextAction Recover { get; set; }
    public DelayNextMove CombatRecover { get; set; }
    public ResetTarget ChoosePatrolPoint { get; set; }
    public Vector2 PreferredRange { get; set; }
    public float PreferredTolerance { get; set; }
    public bool IsVelocityVoid { get; set; } = true;
    public Vector2? SuspiciousSpot { get; set; }

    public float MoveSpeed => StatBlock.GetStatLvlConvertVal(Stats.Speed);
    public float TurnSpeed { get; set; }
    public float PatrolRadius { get; set; }
    public float Persistence { get; set; }
    public float HuntRecoveryTime { get; set; }
    public float MeleeRange { get; set; }
    public GameObject Face { get; set; }
    public Transform LookAtPoint => Face.transform.GetChild(0);
    
    public bool IsResting { get; set; }
    public bool IsStunned { get; set; }
    public bool IsTracking { get; set; }
    public bool IsTargetLost { get; set; }
    public Transform Transform { get; set; }
    public Rigidbody2D Rigidbody { get; set; }
    [SerializeField]
    private List<Transform> spottedTargets = new();
    public List<Transform> SpottedTargets => spottedTargets;
    public StatBlock StatBlock { get; set; }
    public EnemyOrientation EnemyOrientation { get; set; }

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
        }
    }

    public float DistFromTarget => TargetTransform != null ? Vector2.Distance(Transform.position, (Vector2)TargetTransform.position) : -1;
    public float DistFromTargetPos => Vector2.Distance(Transform.position, (Vector2)TargetPos);

    private Vector2 targetPos;
    public Vector2? TargetPos
    {
        get => targetPos;
        set
        {
            targetPos = value ?? (TargetTransform != null ? TargetTransform.position : ChoosePatrolPoint());
        }
    }

    public Vector2? MemorizedTargetPos { get; set; }

    private Quaternion targetRotation;
    public Quaternion TargetRotation { get => targetRotation; set => targetRotation = value; }

    public Vector2 LookAt()
    {
        Vector2 dirToTarget = (Vector2)TargetPos - (Vector2)Face.transform.position;
        float angle = Mathf.Atan2(dirToTarget.x, dirToTarget.y) * Mathf.Rad2Deg;
        targetRotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        Face.transform.rotation = Quaternion.RotateTowards(Face.transform.rotation, targetRotation, TurnSpeed);
        EnemyOrientation.CurrentOrientation = LookAtPoint.position - Face.transform.position;

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
