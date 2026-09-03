using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor (typeof (EntityManager))]
public class EntityManagerEditor : Editor
{
    private EntityProperties props;
    private List<Vector3> points = new();

    private Vector2 dir45 = new(Mathf.Cos(45 * Mathf.Deg2Rad), Mathf.Sin(45 * Mathf.Deg2Rad));
    private Vector2 dir90 = new(Mathf.Cos(90 * Mathf.Deg2Rad), Mathf.Sin(90 * Mathf.Deg2Rad));
    private Vector2 dir135 = new(Mathf.Cos(135 * Mathf.Deg2Rad), Mathf.Sin(135 * Mathf.Deg2Rad));
    private Vector2 dir180 = new(Mathf.Cos(180 * Mathf.Deg2Rad), Mathf.Sin(180 * Mathf.Deg2Rad));
    private Vector2 dir225 = new(Mathf.Cos(225 * Mathf.Deg2Rad), Mathf.Sin(225 * Mathf.Deg2Rad));
    private Vector2 dir270 = new(Mathf.Cos(270 * Mathf.Deg2Rad), Mathf.Sin(270 * Mathf.Deg2Rad));
    private Vector2 dir315 = new(Mathf.Cos(315 * Mathf.Deg2Rad), Mathf.Sin(315 * Mathf.Deg2Rad));
    private Vector2 dir360 = new(Mathf.Cos(360 * Mathf.Deg2Rad), Mathf.Sin(360 * Mathf.Deg2Rad));

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
            VectorToNextPos();
        }

        DrawOrientationCompass();
        ShowNavMeshPath();
        DrawRemainingAngle();
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

    //Triggers error: Look rotation viewing vector is zero, when the LookRotation(Vector3) is (0,0,0). Since we don't use it much, it's commented out
    //and may be removed entirely
    private void VectorToNextPos()
    {
        Vector2 nextPos = props.NavMeshAgent.steeringTarget;
        Vector2 facePos = props.Face.transform.position;
        Vector2 dirToTarget = (nextPos - facePos).normalized;
        Vector2 endPoint = facePos + dirToTarget;

        Handles.color = Color.cyan;
        if(Vector2.Distance(endPoint - facePos, Vector2.zero) > 0.1f)
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

    private void DrawOrientationCompass()
    {
        Handles.color = Color.cyan;

        Handles.DrawLines(new Vector3[]
        {
           props.Transform.position + (Vector3)(dir45 * 0.5f), 
           props.Transform.position + (Vector3)(dir45 * 3.0f), 
           props.Transform.position + (Vector3)(dir90 * 0.5f), 
           props.Transform.position + (Vector3)(dir90 * 3.0f), 
           props.Transform.position + (Vector3)(dir135 * 0.5f), 
           props.Transform.position + (Vector3)(dir135 * 3.0f), 
           props.Transform.position + (Vector3)(dir180 * 0.5f), 
           props.Transform.position + (Vector3)(dir180 * 3.0f), 
           props.Transform.position + (Vector3)(dir225 * 0.5f), 
           props.Transform.position + (Vector3)(dir225 * 3.0f), 
           props.Transform.position + (Vector3)(dir270 * 0.5f), 
           props.Transform.position + (Vector3)(dir270 * 3.0f), 
           props.Transform.position + (Vector3)(dir315 * 0.5f), 
           props.Transform.position + (Vector3)(dir315 * 3.0f), 
           props.Transform.position + (Vector3)(dir360 * 0.5f), 
           props.Transform.position + (Vector3)(dir360 * 3.0f)
        });
    }

    private void DrawRemainingAngle()
    {
        Vector2 origin = props.Transform.position;
        Vector2 faceDir = Quat2Vector(props.Face.transform.rotation);
        Vector2 targetDir = Quat2Vector(props.TargetRotation);
        Handles.color = Color.red;
        Handles.DrawLine(origin + faceDir * 0.5f, origin + faceDir * 10);
        Handles.DrawLine(origin + targetDir * 0.5f, origin + targetDir * 10);
    }

    private Vector2 Quat2Vector(Quaternion quaternion) => quaternion * Vector2.up;
}
