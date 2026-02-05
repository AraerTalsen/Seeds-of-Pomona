using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EvolutionEffect : ScriptableObject
{
    public abstract System.Type PayloadType { get; }
    public void Apply(EvolutionContext context, Payload payload) =>
        HandleEffect(context, payload, true);
    public void Revert(EvolutionContext context, Payload payload) =>
        HandleEffect(context, payload, false);

    protected abstract void HandleEffect(EvolutionContext context, Payload payload, bool isApplied);
}
