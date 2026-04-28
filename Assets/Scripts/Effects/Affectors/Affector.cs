using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public abstract class Affector : IRuntimeFactory
{
    [SerializeField] protected bool lockMovement;
    [SerializeField] protected int stackableUntil;
    
    public enum LifetimeLabel { instant, limited, conditional }
    public enum RepeatLabel { once, continunous, iterate, period, persist }
    public enum TargetLabel { self, single, multiple, environment }

    [SerializeField] protected LifetimeLabel lifetimeLabel;
    [SerializeReference] protected LifetimeLogic lifetime = new InstantLifetime();
    [SerializeField] protected RepeatLabel repeatLabel;
    [SerializeReference] protected RepeatLogic runs = new();
    [SerializeField] protected TargetLabel targetLabel;
    [SerializeField] protected TargetCoordinator coordinator;
    public LifetimeLogic Lifetime => lifetime;
    public LifetimeLabel Label => lifetimeLabel;
    
    public Affector(Affector affector = null)
    {
        if(affector != null) (lifetimeLabel, lifetime, repeatLabel, runs, targetLabel, coordinator) = affector;
    }

    public void Deconstruct
    (
        out LifetimeLabel lifetimeLabel, out LifetimeLogic lifetimeLogic,
        out RepeatLabel repeatLabel, out RepeatLogic repeatLogic,
        out TargetLabel targetLabel, out TargetCoordinator targetCoordinator
    )
    {
        lifetimeLabel = this.lifetimeLabel; lifetimeLogic = lifetime;
        repeatLabel = this.repeatLabel; repeatLogic = runs;
        targetLabel = this.targetLabel; targetCoordinator = coordinator;
    }

    public virtual void Apply(EffectContext context)
    {
        coordinator = new(targetLabel, context);
        if(lockMovement) TogglePauseEntity(context);
    }

    public abstract IRuntimeEvent CreateRuntimeEvent(EffectContext context);

    protected void TogglePauseEntity(EffectContext context)
    {
        if(context.move_Player)
        {
            context.move_Player.TogglePauseMovement();
        }
        else
        {
            EntityProperties props = context.target.gameObject.GetComponent<EntityManager>().EntityProps;
            props.IsStunned = !props.IsStunned;
            props.IsVelocityVoid = !props.IsVelocityVoid;
            props.NavMeshAgent.isStopped = !props.NavMeshAgent.isStopped;
            Rigidbody2D rb = context.target.GetComponent<Rigidbody2D>();
            rb.isKinematic = !rb.isKinematic;
            rb.velocity = Vector2.zero;
        }
    }
}