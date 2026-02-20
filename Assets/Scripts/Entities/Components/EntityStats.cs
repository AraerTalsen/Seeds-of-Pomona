using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    [SerializeField] private StatBlock stats = new();
    public StatBlock Stats { get => stats; }
}
