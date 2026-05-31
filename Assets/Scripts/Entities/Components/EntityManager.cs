using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EntityStateSupport))]
[RequireComponent(typeof(EvolutionTracker))]
public class EntityManager : MonoBehaviour, IStatReact
{
    [SerializeField] private GameObject debugDisplay;
    [SerializeField] private TextMeshProUGUI currentState;
    [SerializeField] private TextMeshProUGUI currentDist;
    [SerializeField] private bool debugState = false;
    [SerializeField] private GameObject stopMarker;
    [SerializeField] private GameObject meleeRange;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float patrolRadius;
    [SerializeField] private float persistence;
    [SerializeField] private float huntRecoveryTime;
    [SerializeField] private GameObject face;

    [SerializeField] private FieldOfView fov;
    public EntityProperties EntityProps { get; set; }

    private EntityStateSupport entityStateSupport;
    
    private NPCEffectContext evolutionContext;
    private EvolutionTracker evolutionTracker;
    [SerializeField] private EntityStats stats;
    //private EffectRunner runner;
    [SerializeField] private EntityOrientation orientation;
    private IRuntimeEvent lastState;
    [SerializeField] private BehaviorLaunchPoint behaviorLaunch;
    
    void Awake()
    {
        EntityProps = new()
        {
            StatBlock = stats.StatBlock,
            TurnSpeed = turnSpeed,
            PatrolRadius = patrolRadius,
            Persistence = persistence,
            HuntRecoveryTime = huntRecoveryTime,
            Transform = transform,
            NavMeshAgent = GetComponent<NavMeshAgent>(),
            Rigidbody = GetComponent<Rigidbody2D>(),
            Orientation = (EnemyOrientation)orientation,
            Face = face,
            MeleeRange = GetComponent<SpriteRenderer>().bounds.size.x + 0.25f,
            PreferredRange = new Vector2(2, 3.5f),
            PreferredTolerance = 1
        };
        entityStateSupport = GetComponent<EntityStateSupport>();
        entityStateSupport.EntityProps = EntityProps;
        fov.EntityProps = EntityProps;
        name = "Enemy " + GetHashCode();
        behaviorLaunch = GetComponent<BehaviorLaunchPoint>();
        behaviorLaunch.Initialize(entityStateSupport, EntityProps);
        evolutionTracker = GetComponent<EvolutionTracker>();
        //runner = GetComponent<EffectRunner>();
        
        evolutionContext = new()
        {
            Owner = new()
            {
                Body = gameObject,
                Worldbox = transform.GetChild(0).GetChild(0).gameObject,
                Stats = stats.StatBlock,
                Orientation = orientation
            }
        };
        evolutionTracker.Context = evolutionContext;

        if(debugState)
        {
            debugDisplay.SetActive(true);
        }

        stats.SubscribeToStatChange(Stats.Speed, this);
        EntityProps.NavMeshAgent.speed = EntityProps.MoveSpeed;

        //EntityProps.NavMeshAgent.updatePosition = false;
        //EntityProps.NavMeshAgent.updateRotation = false;
    }

    //Pass effect to EffectRUnner
    void Update()
    {
        TryRunCurrentState();
        
        if(Input.GetKeyDown(KeyCode.P))
        {
            PrintStateDirectory();
        }
    }

    private async void TryRunCurrentState()
    {
        IRuntimeLauncher launcher = CurrentState();
        
        if(launcher != null)
        {
            try
            {
                IRuntimeEvent effect = await launcher.LaunchEffect(evolutionContext);
                UpdateDebugger(effect);
                
                if(effect != null)
                {
                    EventRunner.Run(effect);
                }
            }
            catch(Exception e)
            {
                Debug.LogError($"Failed to retrieve event: {e.Message}");
            }
        }
    }

    private void UpdateDebugger(IRuntimeEvent effect)
    {
        /*lastState = effect == null || effect.Equals(lastState?.GetType()) ? lastState : effect;
        IBehaviorContext context = ((IBehaviorState)lastState).Context;
        currentState.text = context.GetType().ToString() + "->" + lastState.GetType().ToString();*/

        currentDist.text = EntityProps.DistFromTargetPos.ToString();

        stopMarker.transform.position = (Vector2)EntityProps.TargetPos;

        Vector2 rangeSize = meleeRange.transform.localScale;
        Vector2 rangePos = meleeRange.transform.localPosition;
        meleeRange.transform.localScale = new Vector2(rangeSize.x, EntityProps.MeleeRange);
        meleeRange.transform.localPosition = new Vector2(rangePos.x, EntityProps.MeleeRange / 2);
    }

    private IRuntimeLauncher CurrentState()
    {
        BehaviorState state = behaviorLaunch.BehaviorBase;

        while (state is BehaviorContext context)
        {
            if(context.GetCurrentState() == null) 
            {
                context.SelectNewState();
            }
            state = context.CurrentState;
        }
        return state;
    }

    public void ReactToStatChange(Stats stat)
    {
        EntityProps.NavMeshAgent.speed = EntityProps.MoveSpeed;
    }

    private void OnDisable()
    {
        stats.UnsubscribeFromStatChange(Stats.Speed, this);
    }

    private void PrintStateDirectory()
    {
        BehaviorContext context = behaviorLaunch.BehaviorBase;
        string msg = $"-----{name}'s State Directory-----\n{context}\n{context.Print()}";
        print(msg);
    }
}
