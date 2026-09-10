using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/Effect")]
public class EffectParameters : ScriptableObject
{
    [SerializeField] protected bool lockMovement;
    [SerializeField] protected int stackableUntil;

    [SerializeField] protected PowerUp.EffectLabel effectLabel;
    [SerializeField] protected Affecter.LifetimeLabel lifetimeLabel;
    [SerializeReference] protected LifetimeLogic lifetime = new InstantLifetime();
    [SerializeField] protected Affecter.RepeatLabel repeatLabel;
    [SerializeReference] protected RepeatLogic runs = new();
    [SerializeField] protected Affecter.TargetLabel targetLabel;
    [SerializeField] protected TargetCoordinator coordinator;

    public bool LockMovement { get => lockMovement; set => lockMovement = value; }
    public int StackableUntil { get => stackableUntil; set => stackableUntil = value; }

    public PowerUp.EffectLabel EffectMode => effectLabel;
    public Affecter.LifetimeLabel LifetimeMode { get => lifetimeLabel; set => lifetimeLabel = value; }
    public LifetimeLogic Lifetime { get => lifetime; set => lifetime = value; }
    public Affecter.RepeatLabel RepeatMode { get => repeatLabel; set => repeatLabel = value; }
    public RepeatLogic Runs { get => runs; set => runs = value; }
    public Affecter.TargetLabel TargetMode { get => targetLabel; set => targetLabel = value; }
    public TargetCoordinator Coordinator { get => coordinator; set => coordinator = value; }
    
    public void Deconstruct
    (
        out bool lockMovement, out int stackableUntil,
        out Affecter.LifetimeLabel lifetimeLabel, out LifetimeLogic lifetimeLogic,
        out Affecter.RepeatLabel repeatLabel, out RepeatLogic repeatLogic,
        out Affecter.TargetLabel targetLabel, out TargetCoordinator targetCoordinator
    )
    {
        lockMovement = this.lockMovement; stackableUntil = this.stackableUntil;
        lifetimeLabel = this.lifetimeLabel; lifetimeLogic = lifetime;
        repeatLabel = this.repeatLabel; repeatLogic = runs;
        targetLabel = this.targetLabel; targetCoordinator = coordinator;
    }
    
}
