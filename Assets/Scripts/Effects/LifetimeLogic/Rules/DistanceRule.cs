using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DistanceRule", menuName = "Rules/Distance")]
public class DistanceRule : RuleProcess<float>
{
    [SerializeField] private float threshold = 0.1f;
    protected override float GetValue(EffectContext context)
    {
        return Vector2.Distance(context.owner.transform.position, context.target.position);
    }

    protected override float GetThreshold() => threshold;
}
