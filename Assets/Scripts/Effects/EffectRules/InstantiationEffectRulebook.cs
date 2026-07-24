using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class InstantiationEffectRulebook : IEffectRulebook<InstantiationAffecter>
{
    public static InstantiationEffectRulebook Instance { get;} = new();
    Dictionary<int, Action<EffectContext, InstantiationAffecter>> IEffectRulebook<InstantiationAffecter>.EffectDirectory => new()
    {
        {0, InstantiateNode},
        {1, InstantiateProjectile},
    };

    private void InstantiateNode(EffectContext context, InstantiationAffecter payload)
    {
        GameObject instance = CreateNodeInstance(context, payload);
        instance.GetComponent<EnvironmentalEffect>().StartDecay(payload.NodeDuration);
    }
    
    private void InstantiateProjectile(EffectContext context, InstantiationAffecter payload)
    {
        GameObject instance = CreateNodeInstance(context, payload);

        Vector2 dir = payload.Coordinator.GetDirection(context) * payload.Speed;
        //Later add a stat choice for projectile that can allow a modifier to be selected to act as the force magnitude (on top of base speed)
        instance.GetComponent<Projectile>().FireProjectile(dir, 0);
    }

    private GameObject CreateNodeInstance(EffectContext context, InstantiationAffecter payload)
    {
        GameObject node = payload.Node;
        Vector2 position = payload.IsSpawnAtOrigin ? payload.Coordinator.GetOrigin(context) : payload.Coordinator.GetSpawnPosition(context);
        
        GameObject instance = UnityEngine.Object.Instantiate(node, position, Quaternion.identity);

        if(payload.ParentSelection == InstantiationAffecter.ParentSelect.host)
        {
            instance.transform.parent = context.Owner.Body.transform;
        }
        else if(payload.ParentSelection == InstantiationAffecter.ParentSelect.target)
        {
            instance.transform.parent = context.Targets[0].Body.transform;
        }

        return instance;
    }

    bool[] IEffectRulebook<InstantiationAffecter>.EffectConfig(InstantiationAffecter payload)
    {
        bool[] arr =
        {
            payload.IsProjectile,
        };
        return arr;
    }
}
