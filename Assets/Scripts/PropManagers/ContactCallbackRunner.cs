using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactCallbackRunner : EnvironmentalEffect
{
    public Func<Collider2D, bool> Callback { get; set; }
    private void OnTriggerStay2D(Collider2D other)
    {
        Callback(other);
    }
}
