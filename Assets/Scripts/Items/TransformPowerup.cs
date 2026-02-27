using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Transformation Powerup")]
public class TransformPowerup : Tool
{
    [SerializeField] private TransformEffect effect;

    public override IEffectRuntime CreateEffectRuntime(PowerupContext context) => effect.CreateRuntime(context);
}
