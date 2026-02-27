using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Instantiation Powerup")]
public class InstantiatePowerup : Tool
{
    [SerializeField] private InstantiateEffect effect;

    public override IEffectRuntime CreateEffectRuntime(PowerupContext context) => effect.CreateRuntime(context);
}
