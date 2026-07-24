using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContactRule", menuName = "Rules/Contact")]
public class ContactRule : RuleProcess<int>
{
    [SerializeField] private int threshold;
    protected override int GetThreshold() => threshold;

    protected override int GetValue(EffectContext context) => context.Owner.TactileSense.ActiveCollisions.Count;
}
