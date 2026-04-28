using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantLifetime : LifetimeLogic
{
    public override LifetimeLogic Activate() => (InstantLifetime)Clone();
    public override LifetimeLogic Clone() => new InstantLifetime();
}
