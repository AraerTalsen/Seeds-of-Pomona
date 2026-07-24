using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/States/Config Ability State")]
public class AbilityState : BehaviorState//ScriptableObject, IBehaviorState, IAbilityEffect
{
    private enum ContextType
    {
        Base,
        Investigate,
        Aggro,
        Pursuit,
        Sizing,
        Combat
    }

    private Dictionary<ContextType, System.Type> ContextEnumToType = new()
    {
        {ContextType.Base, typeof(EnemyBehaviorContext)},
        {ContextType.Investigate, typeof(InvestigateState)},
        {ContextType.Aggro, typeof(AggroState)},
        {ContextType.Pursuit, typeof(PursuitState)},
        {ContextType.Sizing, typeof(SizingState)},
        {ContextType.Combat, typeof(CombatState)},
    };
    
    [SerializeField] private ContextType contextType;

    public System.Type HostContext => ContextEnumToType[contextType];
}
