using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BehaviorLaunchPoint : MonoBehaviour
{
    [SerializeField] private EnemyBehaviorContext behaviorBase;
    private EnemyBehaviorContext _runtimeInstance;
    private EnemyBehaviorContext _cachedTemplate;

    public EnemyBehaviorContext BehaviorBase { get => behaviorBase; set => behaviorBase = value; }

    public void Initialize(EntityStateSupport support, EntityProperties props)
    {
        _cachedTemplate = BehaviorBase;
        _runtimeInstance = (EnemyBehaviorContext)DeepCloneNode(BehaviorBase);
        BehaviorBase = _runtimeInstance;

        BehaviorBase.Initialize(support, props);
    }

    public static BehaviorState DeepCloneNode(BehaviorState baseNode, Action<ScriptableObject> callback = null)
    {
        if(baseNode == null) return null;

        BehaviorState clone = Instantiate(baseNode);
        clone.name = clone.name[..clone.name.IndexOf("(")];

        if(clone is BehaviorContext contextClone)
        {
            List<IBehaviorContext.WeightedState> clonedStates = new();
            foreach ((BehaviorState state, int weight) in ((BehaviorContext)baseNode).PossibleStates)
            {
                IBehaviorContext.WeightedState weightedState = new (DeepCloneNode(state, callback), weight);
                clonedStates.Add(weightedState);
            }
            contextClone.PossibleStates = clonedStates;
        }

        callback?.Invoke(clone);

        return clone;
    }

    private void OnDisable()
    {
        BehaviorBase = _cachedTemplate;
    }
}
