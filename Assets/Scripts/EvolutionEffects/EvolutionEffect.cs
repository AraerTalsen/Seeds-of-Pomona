using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EvolutionEffect : ScriptableObject
{
    public abstract System.Type PayloadType { get; }
    public abstract void Apply(EvolutionContext context, Payload payload);
}
