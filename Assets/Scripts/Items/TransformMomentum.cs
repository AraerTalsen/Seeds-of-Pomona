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

    public override IEffectRuntime CreateRuntime(PowerupContext context) => new Runtime(context, this);

    private class Runtime : IEffectRuntime
    {
        private readonly PowerupContext context;
        private readonly TransformMomentum action;

        private float elapsed;

        public bool IsFinished => elapsed >= action.duration;

        public Runtime(PowerupContext context, TransformMomentum action)
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
                context.targetBody.gameObject.GetComponent<Move_Player>().TogglePauseMovement();
            }
        }
    }

    protected override void Apply(PowerupContext context)
    {
        context.targetBody.gameObject.GetComponent<Move_Player>().TogglePauseMovement();
        ApplyMomentum(context);
    }

    private void ApplyMomentum(PowerupContext context)
    {
        Vector2 targetDir = TargetDir(context.stats[Stats.Strength] + force, context.orientation.CurrentOrientation);
        Rigidbody2D rb = context.targetBody.GetComponent<Rigidbody2D>();
        rb.AddForce(targetDir, ForceMode2D.Impulse);
    }
    private Vector2 TargetDir(float magnitude, Vector2 facing) => NormalMoveDir(facing) * magnitude;
    private Vector2 NormalMoveDir(Vector2 facing) => willMoveForward ? facing : -facing;
}