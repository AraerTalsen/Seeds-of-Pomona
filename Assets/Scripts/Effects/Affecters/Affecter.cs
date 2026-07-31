using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public abstract class Affecter<TSelf> : Affecter where TSelf : Affecter<TSelf>
{    
    private static IEffectRulebook<TSelf> _rulebook;

    protected static void RegisterRulebook(IEffectRulebook<TSelf> rulebook)
    {
        _rulebook = rulebook;
    }

    public async override Task Apply(EffectContext context)
    {
        coordinator.Initialize(TargetMode, context);
        if(TargetMode != TargetLabel.self && TargetMode != TargetLabel.environment)
        {
            await Coordinator.ValidateArea(context);
        }
        
        if(lockMovement) TogglePauseEntity(context);
    }

    public override Task<IRuntimeEvent> CreateRuntimeEvent(EffectContext context, Action<EffectContext> callback = null)
    {
        List<Action<EffectContext>> effectCalbacks = new();
        if(lockMovement) 
        {
            effectCalbacks.Add(CallbackWrapper.WrapAction(TogglePauseEntity));
        }
        if(callback != null) effectCalbacks.Add(callback);
        
        return RuntimeEvent<TSelf>.Create(context, (TSelf)this, _rulebook.GetCurrentEffect((TSelf)this), effectCalbacks);
    }

    protected void TogglePauseEntity(EffectContext context)
    {
        if(context.Owner.Worldbox.TryGetComponent(out Move_Player move_player))
        {
            move_player.TogglePauseMovement();
        }
        else
        {
            GameObject target = context.Targets[0].Body;
            EntityProperties props = target.GetComponent<EntityManager>().EntityProps;
            props.IsStunned = !props.IsStunned;
            bool isStopped = !props.NavMeshAgent.enabled || props.NavMeshAgent.isStopped;

            try
            {
                if(!isStopped || (isStopped && props.IsPausingForEffect))
                {
                    //await props.ToggleEntityHalt(!isStopped);
                    props.NavMeshAgent.isStopped = !isStopped;
                    props.Animator.SetFloat("moveMagnitude", 0);
                    props.IsPausingForEffect = !props.IsPausingForEffect;
                }
                
                if(isStopped && props.NavMeshAgent.enabled)
                {
                    Debug.Log("-------------------Warping--------------------");
                    props.NavMeshAgent.Warp(props.NavMeshAgent.transform.position);
                    props.UpdateDestination();
                }
                props.IsVelocityVoid = !props.IsVelocityVoid;
                
                props.NavMeshAgent.updatePosition = !props.NavMeshAgent.updatePosition;
                props.NavMeshAgent.velocity = Vector2.zero;
                Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
                rb.velocity = Vector2.zero;
            }
            catch(Exception e)
            {
                Debug.LogError($"Failed to toggle enemy's halt: {e}");
            }
        }
    }
}

public abstract class Affecter : IRuntimeFactory
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
    public TargetLabel TargetMode => targetLabel;
    public TargetCoordinator Coordinator => coordinator;

    public Affecter(EffectParameters parameters)
    {
        (
            lockMovement, stackableUntil,
            lifetimeLabel, lifetime,
            repeatLabel, runs,
            targetLabel, coordinator
        ) = parameters;    
    }

    public Affecter(Affecter affecter = null)
    {
        if(affecter != null) 
        {
            (
                lockMovement, stackableUntil,
                lifetimeLabel, lifetime,
                repeatLabel, runs,
                targetLabel, coordinator
            ) = affecter;
        }
            
    }

    public void Deconstruct
    (
        out bool lockMovement, out int stackableUntil,
        out LifetimeLabel lifetimeLabel, out LifetimeLogic lifetimeLogic,
        out RepeatLabel repeatLabel, out RepeatLogic repeatLogic,
        out TargetLabel targetLabel, out TargetCoordinator targetCoordinator
    )
    {
        lockMovement = this.lockMovement; stackableUntil = this.stackableUntil;
        lifetimeLabel = this.lifetimeLabel; lifetimeLogic = lifetime;
        repeatLabel = this.repeatLabel; repeatLogic = runs;
        targetLabel = this.targetLabel; targetCoordinator = coordinator;
    }

    public abstract Task Apply(EffectContext context);

    public abstract Task<IRuntimeEvent> CreateRuntimeEvent(EffectContext context, Action<EffectContext> callback = null);
}