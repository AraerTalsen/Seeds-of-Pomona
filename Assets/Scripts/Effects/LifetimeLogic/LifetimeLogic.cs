using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LifetimeLogic
{
    public virtual bool ShouldDestroy() => true;
}
