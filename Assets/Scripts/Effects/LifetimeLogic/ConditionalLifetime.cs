using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ConditionalLifetime : LifetimeLogic
{
    [SerializeField] private List<LifetimeRule> _rules = new();
    public List<LifetimeRule> Rules => _rules;

    public ConditionalLifetime(List<LifetimeRule> rules = null) { _rules = rules; }

    public override bool ShouldDestroy(EffectContext context) => IsConditionMet(context);

    public override LifetimeLogic Activate()
    {
        ConditionalLifetime activated = (ConditionalLifetime)Clone();
        return activated;
    }

    public override LifetimeLogic Clone() => new ConditionalLifetime(Rules);
    public void Apply(List<LifetimeRule> rules) => _rules = rules;

    private bool IsConditionMet(EffectContext context)
    {
        foreach(var rule in Rules)
        {
            if(rule.IsBroken(context)) return true;
        }
        return false;
    }
}
