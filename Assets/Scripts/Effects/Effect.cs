using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Effect
{
    public enum LifetimeLabel { instant, limited, conditional }
    public enum RepeatLabel { once, continunous, iterate, period, persist }
    public enum TargetLabel { self, single, multiple, environment }
    
    public Effect(Effect effect = null)
    {
        if(effect != null) (lifetimeLabel, lifetime, repeatLabel, runs, targetLabel, coordinator) = effect;
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

    [SerializeField] protected LifetimeLabel lifetimeLabel;
    [SerializeReference] protected LifetimeLogic lifetime = new();
    [SerializeField] protected RepeatLabel repeatLabel;
    [SerializeReference] protected RepeatLogic runs = new();
    [SerializeField] protected TargetLabel targetLabel;
    [SerializeField] protected TargetCoordinator coordinator;
    public LifetimeLogic Lifetime => lifetime;
    public LifetimeLabel Label => lifetimeLabel;
}