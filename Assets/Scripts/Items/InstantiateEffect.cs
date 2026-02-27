using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Instantiate Effect")]
public class InstantiateEffect : PowerupEffect
{
    [SerializeField] private GameObject node;
    [SerializeField] private int lifespan;

    public override IEffectRuntime CreateRuntime(PowerupContext context) => new InstantRuntime(context, this);

    private class InstantRuntime : IEffectRuntime
    {
        public bool IsFinished { get; private set; }

        public InstantRuntime(PowerupContext context, InstantiateEffect action)
        {
            action.Apply(context);
            IsFinished = true;
        }

        public void Tick() { }
    }

    protected override void Apply(PowerupContext context)
    {
        GameObject g = Instantiate(node, context.targetBody.position, Quaternion.identity);
        g.GetComponent<EnvironmentalEffect>().StartDecay(lifespan); 
    }
}
