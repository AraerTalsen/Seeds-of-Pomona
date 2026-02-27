using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Instantiate Effect")]
public class InstantiateEffect : PowerupEffect
{
    [SerializeField] protected GameObject node;
    [SerializeField] protected float lifespan;
    [SerializeField] protected bool spawnForward = true;

    public override IEffectRuntime CreateRuntime(PowerupContext context) => new InstantRuntime(context, this);

    private class InstantRuntime : IEffectRuntime
    {
        public bool IsFinished { get; private set; }

        public InstantRuntime(PowerupContext context, InstantiateEffect effect)
        {
            effect.Apply(context);
            IsFinished = true;
        }

        public void Tick() { }
    }

    protected override void Apply(PowerupContext context)
    {
        GameObject g = Instantiate(node, (Vector2)context.targetBody.position + NormalSpawnDir(context.orientation.CurrentOrientation), Quaternion.identity);
        g.GetComponent<EnvironmentalEffect>().StartDecay(lifespan); 
    }

    protected Vector2 NormalSpawnDir(Vector2 facing) => spawnForward ? facing : -facing;
}
