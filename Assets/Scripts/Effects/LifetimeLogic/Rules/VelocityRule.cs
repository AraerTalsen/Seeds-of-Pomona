using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VelocityRule", menuName = "Rules/Velocity")]
public class VelocityRule : RuleProcess<float>
{
    [SerializeField] private float threshold;
    protected override float GetThreshold() => threshold;

    protected override float GetValue(EffectContext context) => context.Targets[0].Body.GetComponent<Rigidbody2D>().velocity.magnitude;
}
