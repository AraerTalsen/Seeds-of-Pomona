using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Aggro")]
public class AggroState : BehaviorContext
{
    private float max;
    private float tolerance;

    public override bool IsAggro => true;

    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }

    private EntityProperties entityProps;
    [BranchCondition] private bool DistMoreTolerance { get; set; }
    public override EntityProperties EntityProps
    {
        get => entityProps;
        set
        {
            entityProps = value;
            ContextRegistry = Context.ContextRegistry;
            AddToRegistry(this);
            InitializeStates();
            CurrentState = PossibleStates.Find(w => w.State is PursuitState).State;
            tolerance = EntityProps.PreferredTolerance;
            max = EntityProps.PreferredRange.y;
        }
    }

    public override void SelectNewState()
    {
        DistMoreTolerance = EntityProps.DistFromTarget > max + tolerance;
        if(DistMoreTolerance)
        {
            CalculateTargetPos();
            CurrentState = PossibleStates.Find(w => w.State is PursuitState).State;
        }
        else
        {
            CurrentState = ChooseRandomState();
        }
    }

    private void CalculateTargetPos()
    {
        float dist = EntityProps.DistFromTarget;
        Vector2 current = EntityProps.Transform.position;        
        Vector2 target = EntityProps.TargetTransform.position;
        Vector2 dirToTarget = (target - current).normalized;
        float magnitude = dist - max - tolerance + 0.1f;
        EntityProps.TargetPos = current + dirToTarget * magnitude;
    }

    public override IBehaviorState GetCurrentState()
    {
        DistMoreTolerance = EntityProps.DistFromTarget > max + tolerance;
        if(DistMoreTolerance)
        {
            CurrentState = null;
        }
        return base.GetCurrentState();
    }
}
