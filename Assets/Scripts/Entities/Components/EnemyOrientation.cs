using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrientation : EntityOrientation
{
    [SerializeField] private List<Sprite> faceDirs = new();
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float prevAngle;

    public override Vector2 CurrentOrientation 
    { 
        get => currentOrientation; 
        set 
        {
            currentOrientation = value.normalized; 
            float angle = DirToAngle(currentOrientation);
            spriteRenderer.sprite = AngleToSprite(angle);
            prevAngle = angle;
        }
    }

    private float DirToAngle(Vector2 dir)
    {
        float signedAngle = Vector2.SignedAngle(Vector2.right, dir);
        return signedAngle < 0 ? signedAngle + 360 : signedAngle;
    }

    private Sprite AngleToSprite(float angle) =>
        angle switch
        {
            < 45 => faceDirs[0],
            < 90 => VerticalSprite,
            < 135 => faceDirs[1],
            < 180 => faceDirs[2],
            < 225 => faceDirs[3],
            < 270 => VerticalSprite,
            < 315 => faceDirs[4],
            < 360 => faceDirs[5],
            _ => faceDirs[0]
        };
    
    private Sprite VerticalSprite => 
        prevAngle switch
        {
            < 45 => faceDirs[1],
            < 135 => faceDirs[0],
            < 225 => faceDirs[4],
            < 315 => faceDirs[3],
            _ => faceDirs[0]
        };
}
