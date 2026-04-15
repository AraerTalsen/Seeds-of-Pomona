using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PUp")]
public class PUp : ScriptableObject
{
    public enum EffectLabel { transform, stat }
    
    [SerializeField] private EffectLabel effectLabel;
    [SerializeReference] public Effect effect = new();
}
