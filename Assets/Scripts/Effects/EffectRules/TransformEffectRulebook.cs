using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformEffectRulebook
{
    public static Action<EffectContext, TransformAffector> GetCurrentEffect(TransformAffector payload)
    {
        bool isPosition = payload.TransformType == TransformAffector.TransformLabel.position;
        bool isInstant = payload.IsInstant;

        //if(isPosition)
        //{
            return isInstant ? SetPosition: TranslatePosition;
        //}
    }
    
    public static void SetPosition(EffectContext context, TransformAffector payload)
    {
        context.target.position = payload.Position;
    }

    public static void TranslatePosition(EffectContext context, TransformAffector payload)
    {
        if(payload.HasTarget && Vector2.Distance(context.target.position, payload.Position) <= 0.1f) return;

        Vector2 convertDir = ConvertDirCoordSytem(payload.Direction, context.orientation.CurrentOrientation);
        Vector2 targetDir = convertDir * payload.Speed;
        Rigidbody2D rb = context.target.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.AddForce(targetDir, ForceMode2D.Impulse);
    }

    public static void SetOrientation(EffectContext context, TransformAffector payload)
    {
        context.target.GetComponent<EntityOrientation>().CurrentOrientation = DegreesToVector2(payload.Angle);
    }

    public static void RotateOrientation(EffectContext context, TransformAffector payload)
    {
        EntityOrientation orientation = context.target.GetComponent<EntityOrientation>();
        Vector2 currOrientation = orientation.CurrentOrientation;

        if(NormalizeAngleDeg(payload.Angle) - MathF.Atan2(currOrientation.y, currOrientation.x) <= 0.1f) return;
        
        if(context.target.TryGetComponent(out EntityManager manager))
        {
            EntityProperties props = manager.EntityProps;
            context.target.Rotate((payload.IsClockwise ? 1 : -1) * payload.Speed * Time.deltaTime * Vector2.right);
            props.EnemyOrientation.CurrentOrientation = props.NavMeshAgent.velocity.magnitude > 0 ? 
                props.NavMeshAgent.velocity.normalized : props.LookAtPoint.position - props.Face.transform.position;
        }
        else
        {            
            float angle = (payload.IsClockwise ? 1 : -1) * payload.Speed * Time.deltaTime;
            Vector2 newOrientation = RotateVector2(orientation.CurrentOrientation, angle);

            context.target.GetComponent<EntityOrientation>().CurrentOrientation = newOrientation;
        }
        
    }

    private static Vector2 DegreesToVector2(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians);
        float y = Mathf.Sin(radians);

        return new Vector2(x, y);
    } 

    private static Vector2 RotateVector2(Vector2 direction, float angle)
    {
        Vector2 vecAngle = DegreesToVector2(angle);

        return new Vector2(
            direction.x * vecAngle.x - direction.y * vecAngle.y,
            direction.x * vecAngle.y + direction.y * vecAngle.x
        ).normalized;
    }

    private static float NormalizeAngleDeg(float angle)
    {
        if(angle > 0 && angle <= 360) return angle;

        angle = Mathf.Abs(angle) > 360 ? angle % 360 : angle;

        return angle < 0 ? 360 - angle : angle;
    }

    private static Vector2 ConvertDirCoordSytem(Vector2 fieldDir, Vector2 worldDir)
    {
        float x = fieldDir.x * worldDir.x - fieldDir.y * worldDir.y;
        float y = fieldDir.x * worldDir.y + fieldDir.y * worldDir.x;

        return new(x, y);
    }
}
