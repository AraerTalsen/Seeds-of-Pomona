using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectContext
{
    public Transform target;//How will we get the target for here?
    public StatBlock stats;
    public EntityOrientation orientation;//It would be more helpful to select the target's orientation
    public GameObject owner;
    public Move_Player move_Player;
}
