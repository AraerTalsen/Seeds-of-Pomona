using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public EntityProperties EntityProps { get; set; }
    public int viewAngle;
    public float viewRadius;

    [HideInInspector]
    public List<Transform> visibleTargets = new();
    private List<Transform> lastTargets = new();

    // Update is called once per frame
    void Update()
    {
        Observe();
        if(EntityProps.TargetTransform != null)
        {
            EntityProps.TargetPos = EntityProps.TargetTransform.position;   
        }
    }

    //Remove from visible targets any entities outside of view radius
    private void Observe()
    {
        List<Collider2D> targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius).ToList();
        visibleTargets.RemoveAll(transform => !targetsInViewRadius.Select(collider => collider.transform).Contains(transform));

        CheckIfTargetVisible(targetsInViewRadius);

        UpdateCurrentTarget();
    }

    private void CheckIfTargetVisible(List<Collider2D> targetsInViewRadius)
    {
        for (int i = 0; i < targetsInViewRadius.Count; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            float distToTarget = Vector2.Distance(transform.position, target.position);
            bool isInList = visibleTargets.Find((t) => t == target);

            UpdateVisibleTarget(target, dirToTarget, distToTarget, isInList);
        }
        CompareVisibleTargets();
    }

    private void UpdateVisibleTarget(Transform target, Vector2 dirToTarget, float distToTarget, bool isInList)
    {
        if (target.CompareTag("Player") && Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up, dirToTarget, distToTarget);
            
            if (hit && hit.collider.CompareTag("Player") && !isInList)
            {
                Move_Player mp = target.gameObject.GetComponent<Move_Player>();
                if (!mp.IsHidden) visibleTargets.Add(target);
            }
            else if(hit && !hit.collider.CompareTag("Player") && isInList)
            {
                visibleTargets.Remove(target);
            }
        }
        else if (isInList)
        {
            visibleTargets.Remove(target);
        }

    }

    private void CompareVisibleTargets()
    {
        if (visibleTargets.Except(lastTargets).ToList().Count > 0)
        {
            UpdateCurrentTarget();
        }
        lastTargets = visibleTargets;
    }

    private void UpdateCurrentTarget()
    {
        if (visibleTargets.Count > 0)
        {
            EntityProps.TargetTransform = visibleTargets[0];
        }
        else
        {
            Vector2 temp = (Vector2)EntityProps.TargetPos;
            EntityProps.TargetTransform = null;
            EntityProps.TargetPos = temp;
        }
    }

    public Vector2 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += -transform.eulerAngles.z;
        float x = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
        float y = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
        return new Vector2(x, y);
    }
}