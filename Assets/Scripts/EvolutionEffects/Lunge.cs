using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Base-Context Ability State")]
public class BaseContextState : AbilityState<BehaviorContext>
{
    [SerializeField]
    private float lungeDist;
    [SerializeField]
    private float lungeSpeed;

    //public override void PerformAction(EffectContext context) => CreateEffectRuntime(context);

    public override IEffectRuntime CreateEffectRuntime(EffectContext context) => effect.CreateRuntime(context);
}
