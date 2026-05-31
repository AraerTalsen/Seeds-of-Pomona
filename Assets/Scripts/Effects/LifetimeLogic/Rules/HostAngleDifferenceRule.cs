using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HostAngleDifferenceRule", menuName = "Rules/Difference/HostAngle")]
public class HostAngleDifferenceRule : RuleProcess<float>
{
    [SerializeField] private float threshold = 5;
    protected override float GetValue(EffectContext context)
    {
        EntityProperties props = context.Targets[0].Body.GetComponent<EntityManager>().EntityProps;
        return Quaternion.Angle(props.Face.transform.rotation, props.TargetRotation);
    }

    protected override float GetThreshold() => threshold;
}
