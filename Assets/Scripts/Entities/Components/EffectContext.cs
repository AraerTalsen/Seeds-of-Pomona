using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectContext
{
    public struct Entity
    {
        public GameObject Body { get; set; }
        public GameObject Worldbox { get; set; }
        public StatBlock Stats { get; set; }
        public EntityOrientation Orientation { get; set; }
    }

    public Entity Owner;
    public List<Entity> Targets = new();

    public void AddTarget(GameObject target)
    {
        if(target.GetComponent<EntityStats>() && Targets.FindIndex(e => e.Body == target) == -1)
        {
            if(target.TryGetComponent(out EntityManager manager))
            {
                EntityProperties props = manager.EntityProps;
                Targets.Add( new()
                {
                    Body = target,
                    Worldbox = target.transform.GetChild(0).GetChild(0).gameObject,
                    Stats = props.StatBlock,
                    Orientation = props.Orientation
                });
            }
            else
            {
                Targets.Add( new()
                {
                    Body = target,
                    Worldbox = target.transform.GetChild(0).GetChild(0).gameObject,
                    Stats = Owner.Stats,
                    Orientation = Owner.Orientation
                });
            }
        }
    }
}
