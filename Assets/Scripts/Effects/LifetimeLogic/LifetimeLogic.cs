using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LifetimeLogic
{
    public virtual bool ShouldDestroy(EffectContext context) => true;
    public abstract LifetimeLogic Activate();
    public abstract LifetimeLogic Clone();
}
