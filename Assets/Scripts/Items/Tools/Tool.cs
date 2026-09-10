using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : Item
{
    public abstract void ToolAction(EffectContext context);
}
