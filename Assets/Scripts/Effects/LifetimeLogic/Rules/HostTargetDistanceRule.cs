using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HostTargetDistanceRule", menuName = "Rules/Distance/HostTarget")]
public class HostTargetDistanceRule : RuleProcess<float>
{
    [SerializeField] private float threshold = 0.1f;
    protected override float GetValue(EffectContext context)
    {
        return Vector2.Distance(context.Owner.Body.transform.position, context.Targets[0].Body.transform.position);
    }

    protected override float GetThreshold() => threshold;
}
