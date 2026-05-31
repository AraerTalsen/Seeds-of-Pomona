using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Instantiate Projectile")]
public class InstantiateProjectile : InstantiateEffect
{
    [SerializeField] private float magnitude;
    [SerializeField] private int damage;

    protected override void Apply(EffectContext context)
    {
        Vector2 dir = NormalSpawnDir(context.Owner.Orientation.CurrentOrientation);
        GameObject g = Instantiate(node, (Vector2)context.Targets[0].Body.transform.position + dir * 1.625f, Quaternion.identity);
        g.GetComponent<Projectile>().FireProjectile(dir * magnitude, damage);
    }
}
