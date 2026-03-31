using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

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
    public NavMeshAgent NavMeshAgent { get; set; }
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
            if(value != null)
            {
                targetPos = ClampToNavMesh((Vector2)value);
            }
            else
            {
                targetPos = TargetTransform != null ? TargetTransform.position : ClampToNavMesh(ChoosePatrolPoint());
            }
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

    private Vector2 ClampToNavMesh(Vector2 origin) => NavMesh.SamplePosition(origin, out _, 0.1f, NavMesh.AllAreas) ? origin : FindNearestOnMesh(origin);

    private Vector2 FindNearestOnMesh(Vector2 origin)
    {
        Vector2[] mods = { Vector2.up, Vector2.right, Vector2.down, Vector2.left }; 
        int maxDist = 10;
        int currentDist = 1;
        List<Vector2> prevChecks = new() { origin };
        List<Vector2> allChecks = new() { origin };
        List<Vector2> curChecks = new();

        while (currentDist < maxDist)
        {
            for (int i = 0; i < prevChecks.Count; i++)
            {
                for (int j = 0; j < mods.Length; j++)
                {
                    Vector2 newPoint = prevChecks[i] + mods[j];

                    if(!allChecks.Contains(newPoint))
                    {
                        curChecks.Add(newPoint);
                        allChecks.Add(newPoint);
                    }

                    bool isWalkable = NavMesh.SamplePosition(newPoint, out _, 0.1f, NavMesh.AllAreas);
                    if (isWalkable)
                    {
                        return newPoint;
                    }
                }
            }
            
            prevChecks = new(curChecks);
            curChecks.Clear();
            
            currentDist++;
        }
        Debug.Log($"NavMesh is not within range of origin: {origin}");
        return origin;
    }
}
