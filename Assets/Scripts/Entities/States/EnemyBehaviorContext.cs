using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Enemy Behavior Base")]
public class EnemyBehaviorContext : BehaviorContext
{    
    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }

    public override float RecoveryTime { get; } = 8.0f;
    public override Dictionary<System.Type, IBehaviorContext> ContextRegistry { get; set; } = new();

    private bool isResting;
    [BranchCondition] private bool IsResting 
    { 
        get => isResting;
        set
        {
            isResting = value;
            IsRestingTimestamp = Time.time;
        }
    }
    [ConditionTimestamp] private float IsRestingTimestamp { get; set; }
    private bool isStunned;
    [BranchCondition] private bool IsStunned 
    { 
        get => isStunned;
        set
        {
            isStunned = value;
            IsStunnedTimestamp = Time.time;
        }
    }
    [ConditionTimestamp] private float IsStunnedTimestamp { get; set; }
    private bool hasSpotted;
    [BranchCondition] private bool HasSpotted 
    { 
        get => hasSpotted;
        set
        {
            hasSpotted = value;
            HasSpottedTimestamp = Time.time;
        }
    }
    [ConditionTimestamp] private float HasSpottedTimestamp { get; set; }
    private bool recallTargetPos;
    [BranchCondition] private bool RecallsTargetPos 
    { 
        get => recallTargetPos;
        set
        {
            recallTargetPos = value;
            RecallsTargetTimestamp = Time.time;
        }
    }
    [ConditionTimestamp] private float RecallsTargetTimestamp { get; set; }

    public void Initialize(EntityStateSupport entityStateSupport, EntityProperties entityProps)
    {
        EntityStateSupport = entityStateSupport;
        EntityProps = entityProps;

        CurrentState = ChooseRandomState();
        InitializeStates();
        IsValidTimestamp = Time.time;
    }

    protected override bool DefaultNodePathValidity(BehaviorState node) => 
        !IsStunned && !IsResting &&
        !HasSpotted && !RecallsTargetPos &&
        node.IsValid;
    public override void InitializeBranchValidityRec()
    {
        BehaviorState aggro = PossibleStates.Find(w => w.State is AggroState).State;
        nodeValidityCheck.Add(aggro, s => !IsStunned && (HasSpotted || RecallsTargetPos) && s.IsValid);
        branchValidity.Add(aggro, nodeValidityCheck[aggro](aggro));

        List<BehaviorState> remainingStates = PossibleStates.Where( w => w.State != aggro).Select( w => w.State).ToList();
        for(int i = 0; i < remainingStates.Count; i++)
        {
            BehaviorState state = remainingStates[i];
            nodeValidityCheck.Add(state, s => DefaultNodePathValidity(s));
            branchValidity.Add(state, nodeValidityCheck[state](state));
        }
    }

    public override async void SelectNewState()
    {
        HasSpotted = EntityStateSupport.CheckForTargetEntities();
        IsStunned = EntityProps.IsStunned;
        IsResting = EntityProps.IsResting;
        RecallsTargetPos = EntityProps.MemorizedTargetPos != null;
        
        if(!IsStunned)
        {
            if(!IsResting && !HasSpotted && !RecallsTargetPos)
            {
                CurrentState = ChooseRandomState();
            }
            else if(HasSpotted || RecallsTargetPos)
            {
                CurrentState = PossibleStates.Find(w => w.State is AggroState).State;
            }
            else if(IsResting)
            {
                CurrentState = null;
                try
                {
                    //await EntityProps.ToggleEntityHalt(true);
                    EntityProps.NavMeshAgent.isStopped = true;
                    EntityProps.Animator.SetFloat("moveMagnitude", 0);
                }
                catch(Exception e)
                {
                    Debug.LogError($"Failed to toggle enemy's halt: {e}");
                }
            }
        }
        else
        {
            CurrentState = null;
            try
            {
                //await EntityProps.ToggleEntityHalt(true);
                EntityProps.NavMeshAgent.isStopped = true;
                EntityProps.Animator.SetFloat("moveMagnitude", 0);
            }
            catch(Exception e)
            {
                Debug.LogError($"Failed to toggle enemy's halt: {e}");
            }
        }
    }

    public override IBehaviorState GetCurrentState()
    {
        ReassessSystem();
        
        return base.GetCurrentState();
    }

    private void ReassessSystem()
    {
        HasSpotted = EntityStateSupport.CheckForTargetEntities();
        IsStunned = EntityProps.IsStunned;
        IsResting = EntityProps.IsResting;

        if(IsStunned || IsResting || HasSpotted)
        {
            CurrentState = null;
        }

        if(HasSpotted)
        {
            EntityProps.SuspiciousSpot = null;
        }
    }
}