using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimer
{
    public int IntervalInDays {get;}
    //Progress state of machine
    public void IncrementTime();
}
