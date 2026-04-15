using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodRepeat : RepeatLogic
{
    [SerializeField] private float period;
    [SerializeField] private float interval;
    private float startTime;
    public override bool IsComplete() => startTime + period < Time.time;
}
