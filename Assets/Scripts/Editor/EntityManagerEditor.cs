using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (EntityManager))]
public class EntityManagerEditor : Editor
{
    private EntityProperties props;
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
}
