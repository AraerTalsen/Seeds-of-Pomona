using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConditionalLifetime : LifetimeLogic
{
    [SerializeField] private LifetimeRule rule;
    public override bool ShouldDestroy() => rule.IsBroken();
}
