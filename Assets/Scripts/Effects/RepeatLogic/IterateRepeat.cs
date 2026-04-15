using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IterateRepeat : RepeatLogic
{
    [SerializeField] private int iterations;
    [SerializeField] private float interval;
    private int currIter = 0;
    public override bool IsComplete() => currIter > iterations;
}
