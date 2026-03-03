using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityStateSupport))]
[RequireComponent(typeof(EvolutionTracker))]
public class EntityManager : MonoBehaviour
{
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
            Face = face
        };

        entityStateSupport = GetComponent<EntityStateSupport>();
        entityStateSupport.EntityProps = EntityProps;
        fov.EntityProps = EntityProps;
        enemyBehaviorContext = new(entityStateSupport, EntityProps);
        evolutionTracker = GetComponent<EvolutionTracker>();
        runner = GetComponent<EffectRunner>();
        
        evolutionContext = new()
        {
            stats = stats.StatBlock,
            stateMachine = enemyBehaviorContext,
            targetBody = transform,
            orientation = orientation
        };
        evolutionTracker.Context = evolutionContext;
    }

    //Pass effect to EffectRUnner
    void Update()
    {
        runner.Run(enemyBehaviorContext, evolutionContext);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerDeathManager>().KillPlayer();
        }
    }
}
