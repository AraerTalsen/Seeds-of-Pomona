using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateSupport : MonoBehaviour
{
    private EntityProperties entityProps;
    public EntityProperties EntityProps
    {
        get => entityProps;
        set
        {
            entityProps = value;
            entityProps.ChoosePatrolPoint = ChoosePatrolPoint;
            entityProps.Recover = Recover;
            entityProps.TargetPos = null;
        }
    }

    public Vector2 ChoosePatrolPoint()
    {
        Vector2 currentPos = EntityProps.Transform.position;
        float patrolRadius = EntityProps.PatrolRadius;

        float randX = Random.Range(currentPos.x - patrolRadius, currentPos.x + patrolRadius + 1);
        float randY = Random.Range(currentPos.y - patrolRadius, currentPos.y + patrolRadius + 1);
        Vector2 targetPos = new(randX, randY);

        return AdjustTargetForObstructions(currentPos, targetPos);
    }

    private Vector2 AdjustTargetForObstructions(Vector2 currentPos, Vector2 targetPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(currentPos, (targetPos - currentPos).normalized);

        float hitDist = Vector2.Distance(hit.point, currentPos);
        float targetDist = Vector2.Distance(targetPos, currentPos);

        return hitDist <= targetDist ? currentPos + (targetPos - currentPos).normalized * (hitDist - 1) : targetPos;
    }

    public void Recover(float recoveryTime)
    {
        StartCoroutine(RecoveryTimer(recoveryTime));
    }

    private IEnumerator RecoveryTimer(float recoveryTime)
    {
        EntityProps.IsResting = true;
        yield return new WaitForSeconds(recoveryTime);
        EntityProps.IsResting = false;
    }

    public bool CheckForTargetEntities()
    {
        print($"Target pos : {EntityProps.TargetPos}");
        if (EntityProps.TargetTransform != null)
        {
            if (EntityProps.IsResting || EntityProps.IsTracking)
            {
                StopAllCoroutines();
                EntityProps.IsResting = false;
                EntityProps.IsTracking = false;
            }
            return true;
        }
        else if (EntityProps.IsTargetLost && EntityProps.TargetPos == null)
        {
            //EntityProps.TargetPos = null;
            EntityProps.IsTracking = true;
            EntityProps.IsTargetLost = false;
            StartCoroutine(nameof(TrackTarget));
        }
        return false;
    }

    private IEnumerator TrackTarget()
    {
        yield return new WaitForSeconds(EntityProps.Persistence);
        EntityProps.IsTracking = false;
        Recover(EntityProps.HuntRecoveryTime);
    }
}
