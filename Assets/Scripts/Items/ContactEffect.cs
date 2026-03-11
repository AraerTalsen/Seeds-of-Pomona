using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContactEffect : InstantiateEffect
{
    [SerializeField] private bool debugShowContactBounds = false;
    protected override void Apply(EffectContext context)
    {
        GameObject g = new()
        {
            layer = 2
        };
        g.AddComponent<BoxCollider2D>();
        g.GetComponent<BoxCollider2D>().isTrigger = true;
        g.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
        g.AddComponent<ContactCallbackRunner>();
        g.GetComponent<ContactCallbackRunner>().Callback = Callback;
        g.GetComponent<EnvironmentalEffect>().StartDecay(lifespan);
        
        float range = context.owner.GetComponent<BoxCollider2D>().size.x;
        g.transform.position = (Vector2)context.targetBody.position + NormalSpawnDir(context.orientation.CurrentOrientation) * range;

        if(debugShowContactBounds)
        {
            SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("Sprites/Square");
        }

        node = g;
        //base.Apply(context);
    }

    protected abstract bool Callback(Collider2D other);
}
