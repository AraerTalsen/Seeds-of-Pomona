using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EvolutionEffect : ScriptableObject
{
    public abstract System.Type PayloadType { get; }
    public void Apply(NPCEffectContext context, Payload payload) => HandleEffect(context, payload, true);
    public void Revert(NPCEffectContext context, Payload payload) => HandleEffect(context, payload, false);

    protected abstract void HandleEffect(NPCEffectContext context, Payload payload, bool isApplied);
}
