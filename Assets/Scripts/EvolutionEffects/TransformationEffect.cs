using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/TransformationEffect")]
public class TransformationEffect : EvolutionEffect
{
    public override System.Type PayloadType => typeof(TransformationPayload);
    [SerializeField] private StatEvolutionEffect statEvolutionEffect;
    [SerializeField] private AbilityEvolutionEffect abilityEvolutionEffect;

    protected override void HandleEffect(EvolutionContext context, Payload payload, bool isApplied)
    {
        TransformationPayload data = (TransformationPayload)payload;
        if(isApplied)
        {
            statEvolutionEffect.Apply(context, data.statPayload);
            abilityEvolutionEffect.Apply(context, data.abilityPayload);
        }
        else
        {
            statEvolutionEffect.Revert(context, data.statPayload);
            abilityEvolutionEffect.Revert(context, data.abilityPayload);
        }
        
        VisualTransformation(context, data.scaleIncrease, isApplied);
    }

    private void VisualTransformation(EvolutionContext context, int scale, bool isApplied)
    {
        float changeScale = isApplied ? scale : 1f / scale;
        context.visualEntity.transform.localScale *= changeScale;
    }
}
