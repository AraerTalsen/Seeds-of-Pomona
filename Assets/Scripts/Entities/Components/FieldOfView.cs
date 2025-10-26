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

    private float lastTargetCount = 0;

    // Update is called once per frame
    void Update()
    {
        Observe();
    }

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

            UpdateVisibleTargets(target, dirToTarget, distToTarget, isInList);
        }
    }

    private void UpdateVisibleTargets(Transform target, Vector2 dirToTarget, float distToTarget, bool isInList)
    {
        if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, distToTarget);
            if (hit && target.CompareTag("Player") && !isInList)
            {
                Move_Player mp = target.gameObject.GetComponent<Move_Player>();
                if (!mp.IsHidden) visibleTargets.Add(target);
            }
        }
        else if (isInList)
        {
            visibleTargets.Remove(target);
        }

    }

    private void UpdateCurrentTarget()
    {
        if (lastTargetCount != visibleTargets.Count)
        {
            if (visibleTargets.Count > 0)
            {
                EntityProps.TargetTransform = visibleTargets[0];
            }
            else
            {
                EntityProps.TargetTransform = null;
            }
        }
        lastTargetCount = visibleTargets.Count;
    }

    public Vector2 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += -transform.eulerAngles.z;
        float x = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
        float y = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
        return new Vector2(x, y);
    }
}