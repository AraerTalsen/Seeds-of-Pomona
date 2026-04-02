using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor (typeof (EntityManager))]
public class EntityManagerEditor : Editor
{
    private EntityProperties props;
    private List<Vector3> points = new();
    private void OnSceneGUI()
    {
        if (Event.current.type != EventType.Repaint) return;

        EntityManager manager = (EntityManager)target;
        props = manager.EntityProps;
        if (props == null) return;

        Vector2? targetPosNullable = props.TargetPos;
        if(targetPosNullable != null)
        {
            Vector2 targetPos = (Vector2)targetPosNullable;
            LabelTargetPos(targetPos);
        }

        Vector2? observePosNullable = props?.ObservePoint;
        if(observePosNullable != null)
        {
            Vector2 observePos = (Vector2)observePosNullable;
            LabelObservePos(observePos);
            VectorToTarget(observePos);
        }

        ShowNavMeshPath();
    }

    private void LabelTargetPos(Vector2 center)
    {
        Vector3[] points = new Vector3[]
        {
            center + new Vector2(-0.35f, 0.35f),
            center + new Vector2(0.35f, -0.35f),
            center + new Vector2(0.35f, 0.35f),
            center + new Vector2(-0.35f, -0.35f),
        };

        Handles.color = Color.red;
        Handles.DrawLines(points);
        Handles.DrawWireArc(center, Vector3.forward, Vector2.up, 360, 0.35f, 2);
    }

    private void LabelObservePos(Vector2 pos)
    {
        Handles.color = Color.cyan;
        Handles.DrawSolidDisc(pos, Vector3.forward, 0.35f);
        Handles.DrawWireArc(pos, Vector3.forward, Vector2.up, 360, 0.5f, 2);
    }

    private void VectorToTarget(Vector2 targetPos)
    {
        Vector2 facePos = (Vector2)props.Face.transform.position;
        Vector2 dirToTarget = (targetPos - facePos).normalized;
        Vector2 endPoint = facePos + dirToTarget;

        Handles.color = Color.cyan;
        Handles.ArrowHandleCap(0, facePos, Quaternion.LookRotation(endPoint - facePos), 1, EventType.Repaint);
    }

    private void ShowNavMeshPath()
    {
        NavMeshPath path = props.NavMeshAgent.path;
        
        if(path != null && path.corners.Length > 1)
        {
            Handles.color = Color.cyan;
            points.Clear();
            Handles.DrawWireDisc(path.corners[0], Vector3.forward, 0.35f);

            for(int i = 1; i < path.corners.Length; i++)
            {
                Vector3 corner = path.corners[i];
                Handles.DrawWireDisc(corner, Vector3.forward, 0.35f);
                points.Add(corner);
                points.Add(path.corners[i - 1]);
            }
            Handles.DrawDottedLines(points.ToArray(), 1);
        }
    }
}
