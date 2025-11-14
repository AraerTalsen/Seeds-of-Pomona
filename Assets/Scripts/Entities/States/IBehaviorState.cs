using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviorState
{
    /*public class BehaviorProperties
    {
        public delegate Vector3 ResetTarget();
        public delegate void DelayNextAction(float recoveryTime);

        public DelayNextAction Recover { get; set; }
        public ResetTarget ChoosePatrolPoint { get; set; }
        public float MoveSpeed { get; set; }
        public float TurnSpeed { get; set; }
        public bool IsResting { get; set; }
        public bool IsTracking { get; set; }
        public Transform EntityTrans { get; set; }

        private Vector2? targetPos;
        public Vector2? TargetPos
        {
            get => targetPos;
            set
            {
                targetPos = value == null ? ChoosePatrolPoint() : value;
            }
        }
    }*/

    public IBehaviorContext Context { get; set; }
    public EntityProperties EntityProps { get; set; }
    public EntityStateSupport EntityStateSupport { get; set; }
    public float RecoveryTime { get; }
    public void PerformAction();
}
