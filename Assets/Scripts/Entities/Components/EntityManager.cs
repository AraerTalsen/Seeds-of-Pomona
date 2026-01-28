using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
[RequireComponent(typeof(EntityStateSupport))]
public class EntityManager : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float patrolRadius;
    [SerializeField]
    private float persistence;
    [SerializeField]
    private float huntRecoveryTime;

    public FieldOfView FOV { get; set; }
    public EntityProperties EntityProps { get; set; }

    private EntityStateSupport entityStateSupport;
    private EnemyBehaviorContext enemyBehaviorContext;

    void Awake()
    {
        EntityProps = new()
        {
            MoveSpeed = moveSpeed,
            TurnSpeed = turnSpeed,
            PatrolRadius = patrolRadius,
            Persistence = persistence,
            HuntRecoveryTime = huntRecoveryTime,
            Transform = transform
        };

        entityStateSupport = GetComponent<EntityStateSupport>();
        entityStateSupport.EntityProps = EntityProps;
        FOV = GetComponent<FieldOfView>();
        FOV.EntityProps = EntityProps;
        enemyBehaviorContext = new(entityStateSupport, EntityProps);
    }

    void Update()
    {
        enemyBehaviorContext.PerformAction();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerDeathManager>().KillPlayer();
        }
    }
}
