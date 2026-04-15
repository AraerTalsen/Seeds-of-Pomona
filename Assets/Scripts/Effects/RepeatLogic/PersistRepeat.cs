using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistRepeat : RepeatLogic
{
    [SerializeField] private float interval;
    public override bool IsComplete() => false;
}
