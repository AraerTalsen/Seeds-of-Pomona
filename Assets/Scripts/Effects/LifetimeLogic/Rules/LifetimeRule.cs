using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class LifetimeRule : ScriptableObject
{
    public abstract bool IsBroken(EffectContext context);
}
