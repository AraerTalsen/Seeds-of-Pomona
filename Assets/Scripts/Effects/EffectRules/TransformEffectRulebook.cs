using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class TransformEffectRulebook : IEffectRulebook<TransformAffecter>
{
    public static TransformEffectRulebook Instance { get; } = new();
    Dictionary<int, Action<EffectContext, TransformAffecter>> IEffectRulebook<TransformAffecter>.EffectDirectory => new()
    {
        {1, ApplyLinearForce},
        {2, ApplyRotationalForce},
        {9, TranslatePosition},
        {10, RotateOrientation},
        {13, SetPosition},
        {14, SetOrientation},
    };

    private void SetPosition(EffectContext context, TransformAffecter payload)
    {
        context.Targets[0].Body.transform.position = payload.Position;
    }

    private void TranslatePosition(EffectContext context, TransformAffecter payload)
    {
        
    }

    private void  ApplyLinearForce(EffectContext context, TransformAffecter payload)
    {
        //if(payload.HasTarget && Vector2.Distance(context.target.position, payload.Position) <= 0.1f) return;

        Vector2 convertDir = ConvertDirCoordSytem(payload.Direction, context.Owner.Orientation.CurrentOrientation);
        Vector2 targetDir = convertDir * payload.Speed;
        Rigidbody2D rb = context.Targets[0].Body.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.AddForce(targetDir, ForceMode2D.Impulse);
    }

    private void SetOrientation(EffectContext context, TransformAffecter payload)
    {
        context.Targets[0].Orientation.CurrentOrientation = DegreesToVector2(payload.Angle);
    }

    private void RotateOrientation(EffectContext context, TransformAffecter payload)
    {
        EntityOrientation orientation = context.Targets[0].Orientation;
        Vector2 currOrientation = orientation.CurrentOrientation;

        bool isClockwise = payload.IsClockwise;
        if(payload.IsOptimal)
        {
            isClockwise = NormalizeAngleDeg(payload.Angle) < 180;
        }
        
        if(context.Targets[0].Body.TryGetComponent(out EntityManager manager))
        {
            EntityProperties props = manager.EntityProps;
            context.Targets[0].Body.transform.Rotate((isClockwise ? 1 : -1) * payload.Speed * Time.deltaTime * Vector2.right);
            props.Orientation.CurrentOrientation = props.NavMeshAgent.velocity.magnitude > 0 ? 
                props.NavMeshAgent.velocity.normalized : props.LookAtPoint.position - props.Face.transform.position;
        }
        else
        {            
            float angle = (isClockwise ? 1 : -1) * payload.Speed * Time.deltaTime;
            Vector2 newOrientation = RotateVector2(orientation.CurrentOrientation, angle);

            context.Targets[0].Orientation.CurrentOrientation = newOrientation;
        }
    }

    private void ApplyRotationalForce(EffectContext context, TransformAffecter payload)
    {
        

    }



    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////             Helper Functions             //////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    bool[] IEffectRulebook<TransformAffecter>.EffectConfig(TransformAffecter payload)
    {
        bool[] arr =
        {
            payload.TransformType == TransformAffecter.TransformLabel.position,
            payload.TransformType == TransformAffecter.TransformLabel.rotation,
            payload.IsInstant,
            payload.IsKinematic
        };
        return arr;
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
