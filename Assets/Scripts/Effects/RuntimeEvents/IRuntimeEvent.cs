using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IRuntimeEvent
{
    public Object Owner { get; set; }
    public bool IsFinished { get; }
    public void Tick();
}
