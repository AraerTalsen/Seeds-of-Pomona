using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrientation : EntityOrientation
{
    public EntityProperties EntityProps { get; set; }
    private Vector2[] animParams =
    {
        new (1, 1),
        new (-1, 1),
        new (-1, 0),
        new (-1, -1),
        new (1, -1),
        new (1, 0)
    };
    //[SerializeField] private List<Sprite> faceDirs = new();
    //[SerializeField] private SpriteRenderer spriteRenderer;

    private float prevAngle;

    public override Vector2 CurrentOrientation 
    { 
        get => currentOrientation; 
        set 
        {
            currentOrientation = value.normalized; 
            float angle = DirToAngle(currentOrientation);
            //spriteRenderer.sprite = AngleToSprite(angle);
            Vector2 animPrams = AngleToAnimParams(angle);
            EntityProps.Animator.SetFloat("xDir", animPrams.x);
            EntityProps.Animator.SetFloat("yDir", animPrams.y);
            prevAngle = angle;
        }
    }

    private float DirToAngle(Vector2 dir)
    {
        float signedAngle = Vector2.SignedAngle(Vector2.right, dir);
        return signedAngle < 0 ? signedAngle + 360 : signedAngle;
    }

    private Vector2 AngleToAnimParams(float angle) =>
        angle switch
        {
            < 45 => animParams[0],
            < 90 => VerticalSprite,
            < 135 => animParams[1],
            < 180 => animParams[2],
            < 225 => animParams[3],
            < 270 => VerticalSprite,
            < 315 => animParams[4],
            < 360 => animParams[5],
            _ => animParams[5]
        };
    
    private Vector2 VerticalSprite => 
        prevAngle switch
        {
            < 45 => animParams[1],
            < 135 => animParams[0],
            < 225 => animParams[4],
            < 315 => animParams[3],
            _ => animParams[0]
        };
}
