using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRuntimeEvent
{
    public bool IsFinished { get; }
    public void Tick();
}
