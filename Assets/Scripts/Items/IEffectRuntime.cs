using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectRuntime
{
    public bool IsFinished { get; }
    public abstract void Tick();
}
