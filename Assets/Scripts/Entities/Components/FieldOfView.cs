using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private Collider2D[] ignoreSelf = {};
    public EntityProperties EntityProps { get; set; }
    public int viewAngle;
    public float viewRadius;
    public float faceDist = 0.25f;

    [HideInInspector]
    public List<Transform> visibleTargets = new();
    public List<Collider2D> targetsInViewRadius;
    private List<Transform> lastTargets = new();

    // Update is called once per frame
    void Update()
    {
        Observe();
    }

    //Remove from visible targets any entities outside of view radius
    private void Observe()
    {
        targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, LayerMask.GetMask("Hurtbox")).ToList();
        targetsInViewRadius.RemoveAll( col => col.gameObject.CompareTag("ValidationArea"));
        visibleTargets.RemoveAll( transform => !targetsInViewRadius.Select(collider => collider.transform).Contains(transform));
        
        CheckIfTargetVisible(targetsInViewRadius);

        UpdateCurrentTarget();
    }

    private void CheckIfTargetVisible(List<Collider2D> targetsInViewRadius)
    {
        for (int i = 0; i < targetsInViewRadius.Count; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector2 dirToTarget = (target.position - (transform.position + transform.up * faceDist)).normalized;
            float distToTarget = Vector2.Distance(transform.position, target.position);
            bool isInList = visibleTargets.Find((t) => t == target);

            UpdateVisibleTarget(target, dirToTarget, distToTarget, isInList);
        }
        CompareVisibleTargets();
    }

    private void UpdateVisibleTarget(Transform target, Vector2 dirToTarget, float distToTarget, bool isInList)
    {
        if (target.CompareTag("Player") && Vector2.Angle(transform.up * faceDist, dirToTarget) < viewAngle / 2)
        {
            RaycastHit2D hit = RaycastIgnoreSelf(transform.position + transform.up * faceDist, dirToTarget, distToTarget);
            if (hit && hit.collider.CompareTag("Player") && !isInList)
            {
                GameObject player = target.gameObject;
                Move_Player mp = player.GetComponent<Move_PlayerConnector>().Move_Player;
                DifficultyScaler ds = player.GetComponent<DifficultyScaler>();
                if (!mp.IsHidden) 
                {
                    visibleTargets.Add(target);
                    
                    if(!EntityProps.SpottedTargets.Contains(target))
                    {
                        ds.TimesSpotted++;
                        EntityProps.SpottedNewTarget(target);
                    }
                }
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

    private RaycastHit2D RaycastIgnoreSelf(Vector2 origin, Vector2 dir, float dist)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, dist);
        foreach(RaycastHit2D hit in hits)
        {
            if(System.Array.Exists(ignoreSelf, c => c == hit.collider)) continue;
            else return hit;
        }
        return default;
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
        Transform currentTarget = visibleTargets.Count > 0 ? visibleTargets[0] : null;
        if(currentTarget != EntityProps.TargetTransform)
            EntityProps.TargetTransform = currentTarget;
        
        if(currentTarget != null)
            EntityProps.MemorizedTargetPos = currentTarget.position;
            //This is both in EntityProperties>TargetTransform.set and here. Do we really need it in both?
    }

    public Vector2 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += -transform.eulerAngles.z;
        float x = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
        float y = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
        return new Vector2(x, y);
    }
}