using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(EntityStateSupport))]
[RequireComponent(typeof(EvolutionTracker))]
public class EntityManager : MonoBehaviour
{
    [SerializeField] private GameObject debugDisplay;
    [SerializeField] private TextMeshProUGUI currentState;
    [SerializeField] private TextMeshProUGUI currentDist;
    [SerializeField] private bool debugState = false;
    [SerializeField] private GameObject stopMarker;
    [SerializeField] private GameObject meleeRange;
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float patrolRadius;
    [SerializeField]
    private float persistence;
    [SerializeField]
    private float huntRecoveryTime;
    [SerializeField] private GameObject face;

    [SerializeField] private FieldOfView fov;
    public EntityProperties EntityProps { get; set; }

    private EntityStateSupport entityStateSupport;
    private EnemyBehaviorContext enemyBehaviorContext;
    private NPCEffectContext evolutionContext;
    private EvolutionTracker evolutionTracker;
    private EntityStats stats;
    private EffectRunner runner;
    private EntityOrientation orientation;
    private IAbilityEffect lastState;
    

    void Awake()
    {
        stats = GetComponent<EntityStats>();
        orientation = GetComponent<EnemyOrientation>();
        EntityProps = new()
        {
            StatBlock = stats.StatBlock,
            TurnSpeed = turnSpeed,
            PatrolRadius = patrolRadius,
            Persistence = persistence,
            HuntRecoveryTime = huntRecoveryTime,
            Transform = transform,
            Rigidbody = GetComponent<Rigidbody2D>(),
            EnemyOrientation = (EnemyOrientation)orientation,
            Face = face,
            MeleeRange = GetComponent<SpriteRenderer>().bounds.size.x + 0.25f,
            PreferredRange = new Vector2(2, 3.5f),
            PreferredTolerance = 1
        };

        entityStateSupport = GetComponent<EntityStateSupport>();
        entityStateSupport.EntityProps = EntityProps;
        fov.EntityProps = EntityProps;
        name = "Enemy " + GetHashCode();
        enemyBehaviorContext = new(entityStateSupport, EntityProps);
        evolutionTracker = GetComponent<EvolutionTracker>();
        runner = GetComponent<EffectRunner>();
        
        evolutionContext = new()
        {
            stats = stats.StatBlock,
            stateMachine = enemyBehaviorContext,
            targetBody = transform,
            orientation = orientation,
            owner = gameObject
        };
        evolutionTracker.Context = evolutionContext;

        if(debugState)
        {
            debugDisplay.SetActive(true);
        }
    }

    //Pass effect to EffectRUnner
    void Update()
    {
        IAbilityEffect effect = CurrentState();
        UpdateDebugger(effect);
        
        if(effect != null)
        {
            runner.Run(effect, evolutionContext);
        }
    }

    private void UpdateDebugger(IAbilityEffect effect)
    {
        lastState = effect == null || effect.Equals(lastState?.GetType())  ? lastState : effect;
        IBehaviorContext context = ((IBehaviorState)lastState).Context;
        currentState.text = context.GetType().ToString() + "->" + lastState.GetType().ToString();

        currentDist.text = EntityProps.DistFromTargetPos.ToString();

        stopMarker.transform.position = (Vector2)EntityProps.TargetPos;

        Vector2 rangeSize = meleeRange.transform.localScale;
        Vector2 rangePos = meleeRange.transform.localPosition;
        meleeRange.transform.localScale = new Vector2(rangeSize.x, EntityProps.MeleeRange);
        meleeRange.transform.localPosition = new Vector2(rangePos.x, EntityProps.MeleeRange / 2);
    }

    private IAbilityEffect CurrentState()
    {
        IBehaviorState state = enemyBehaviorContext;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerDeathManager>().KillPlayer();
        }
    }
}
