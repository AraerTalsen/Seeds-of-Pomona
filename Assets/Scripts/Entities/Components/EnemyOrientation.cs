using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrientation : EntityOrientation
{
    [SerializeField] private List<Sprite> faceDirs = new();
    [SerializeField] private SpriteRenderer spriteRenderer;

    public override Vector2 CurrentOrientation 
    { 
        get => currentOrientation; 
        set 
        {
            print(value);
            currentOrientation = value.normalized; 
            spriteRenderer.sprite = AngleToSprite(DirToAngle(currentOrientation));
        }
    }

    private float DirToAngle(Vector2 dir)
    {
        float signedAngle = Vector2.SignedAngle(dir, Vector2.up);
        return signedAngle < 0 ? signedAngle + 360 : signedAngle;
    }

    private Sprite AngleToSprite(float angle) =>
        angle switch
        {
            < 45 => faceDirs[4],
            < 90 => faceDirs[3],
            < 135 => faceDirs[2],
            < 180 => faceDirs[1],
            < 225 => faceDirs[0],
            < 270 => faceDirs[7],
            < 315 => faceDirs[6],
            < 360 => faceDirs[5],
            _ => faceDirs[0]
        };
    
}
