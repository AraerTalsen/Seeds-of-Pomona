using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuleProcess<T> : LifetimeRule where T : IComparable<T>
{
    public enum ComparisonMode { EQUALS, NOT_EQUALS, LESS_THAN, LESS_THAN_EQUAL, GREATER_THAN_EQUAL, GREATER_THAN }
    [SerializeField] protected ComparisonMode comparisonMode;
    protected Dictionary<ComparisonMode, Func<T, T, bool>> compareLookup = new()
    {
        { ComparisonMode.EQUALS, (a, b) => EqualityComparer<T>.Default.Equals(a, b) },
        { ComparisonMode.NOT_EQUALS, (a, b) => !EqualityComparer<T>.Default.Equals(a, b) },
        { ComparisonMode.LESS_THAN, (a, b) => a.CompareTo(b) < 0 },
        { ComparisonMode.LESS_THAN_EQUAL, (a, b) => a.CompareTo(b) <= 0 },
        { ComparisonMode.GREATER_THAN_EQUAL, (a, b) => a.CompareTo(b) >= 0 },
        { ComparisonMode.GREATER_THAN, (a, b) => a.CompareTo(b) > 0 }
    };

    protected abstract T GetValue(EffectContext context);
    protected abstract T GetThreshold();

    public override bool IsBroken(EffectContext context)
    {
        return compareLookup[comparisonMode](GetValue(context), GetThreshold());
    }
}
