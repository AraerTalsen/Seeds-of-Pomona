using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LifetimeRule : ScriptableObject
{
    public abstract bool IsBroken();
}
