using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Momentum Effect")]
public class TransformMomentum : TransformEffect
{
    [SerializeField] private bool willMoveForward = true;
    [SerializeField] private float force;
    [SerializeField] private float duration;

    public Action CallbackOnTick { get; }
    public Action CallbackOnFinished { get; }

    public override IEffectRuntime CreateRuntime(EffectContext context) => new Runtime(context, this);

    private class Runtime : IEffectRuntime
    {
        private readonly EffectContext context;
        private readonly TransformMomentum action;

        private float elapsed;

        public string EffectName => "TransformMomentum";
        public bool IsFinished => elapsed >= action.duration;

        public Runtime(EffectContext context, TransformMomentum action)
        {
            this.context = context;
            this.action = action;

            this.action.Apply(this.context);
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;
            elapsed += deltaTime;

            if(elapsed >= action.duration)
            {
                if(context.move_Player)
                {
                    context.move_Player.TogglePauseMovement();
                }
                else
                {
                    EntityProperties props = context.targetBody.gameObject.GetComponent<EntityManager>().EntityProps;
                    props.IsStunned = false;
                    props.IsVelocityVoid = true;
                    props.NavMeshAgent.isStopped = false;
                }
            }
            action.CallbackOnTick?.Invoke();

            if(IsFinished)
            {
                action.CallbackOnFinished?.Invoke();
            }
        }
    }

    protected override void Apply(EffectContext context)
    {
        if(context.move_Player != null)
        {
            context.move_Player.TogglePauseMovement();
        }
        else
        {
            EntityProperties props = context.targetBody.gameObject.GetComponent<EntityManager>().EntityProps;
            props.IsStunned = true;
            props.IsVelocityVoid = false;
            props.NavMeshAgent.isStopped = true;
        }
        
        ApplyMomentum(context);
    }

    private void ApplyMomentum(EffectContext context)
    {
        Vector2 targetDir = TargetDir(context.stats.GetStatLvlConvertVal(Stats.Strength) + force, context.orientation.CurrentOrientation);
        Rigidbody2D rb = context.targetBody.GetComponent<Rigidbody2D>();
        rb.AddForce(targetDir, ForceMode2D.Impulse);
    }
    private Vector2 TargetDir(float magnitude, Vector2 facing) => NormalMoveDir(facing) * magnitude;
    private Vector2 NormalMoveDir(Vector2 facing) => willMoveForward ? facing : -facing;
}