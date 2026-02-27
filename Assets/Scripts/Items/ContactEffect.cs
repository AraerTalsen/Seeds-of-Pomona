using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContactEffect : InstantiateEffect
{
    [SerializeField] private Sprite sprite;
    protected override void Apply(PowerupContext context)
    {
        GameObject g = new();
        g.AddComponent<BoxCollider2D>();
        g.GetComponent<BoxCollider2D>().isTrigger = true;
        g.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
        g.AddComponent<ContactCallbackRunner>();
        g.GetComponent<ContactCallbackRunner>().Callback = Callback;
        g.GetComponent<EnvironmentalEffect>().StartDecay(lifespan);
        g.transform.position = (Vector2)context.targetBody.position + NormalSpawnDir(context.orientation.CurrentOrientation);
        node = g;
        //base.Apply(context);
    }

    protected abstract bool Callback(Collider2D other);
}
