using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HostPosDistanceRule", menuName = "Rules/Distance/HostPos")]
public class HostPosDistanceRule : RuleProcess<float>
{
    [SerializeField] private float threshold = 0.1f;
    protected override float GetValue(EffectContext context)
    {
        Vector2 targetPos = (Vector2)context.Targets[0].Body.GetComponent<EntityManager>().EntityProps.TargetPos;
        return Vector2.Distance(context.Owner.Body.transform.position, targetPos);
    }

    protected override float GetThreshold() => threshold;
}
