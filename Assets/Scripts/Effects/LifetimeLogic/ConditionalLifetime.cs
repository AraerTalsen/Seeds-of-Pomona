using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConditionalLifetime : LifetimeLogic
{
    [SerializeField] private LifetimeRule _rule;
    public LifetimeRule Rule => _rule;

    public ConditionalLifetime(LifetimeRule rule = null) { _rule = rule; }

    public override bool ShouldDestroy(EffectContext context) => Rule != null && Rule.IsBroken(context);

    public override LifetimeLogic Activate()
    {
        ConditionalLifetime activated = (ConditionalLifetime)Clone();
        return activated;
    }

    public override LifetimeLogic Clone() => new ConditionalLifetime(Rule);
    public void Apply(LifetimeRule rule) => _rule = rule;
}
