using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDefinitions : MonoBehaviour
{
    public class StatDefinition
    {
        public Stats Type { get; }
        public float Multiplier { get; }

        public StatDefinition(Stats type, float multiplier)
        {
            Type = type;
            Multiplier = multiplier;
        }

        public float Apply(float rawValue) => rawValue * Multiplier;
    }

    private static readonly Dictionary<Stats, StatDefinition> _definitions =
        new()
        {
            { Stats.Strength, new StatDefinition(Stats.Strength, 1.0f) },
            { Stats.Speed, new StatDefinition(Stats.Speed, 0.75f) },
            { Stats.Health, new StatDefinition(Stats.Health, 2.0f) },
        };

    public static StatDefinition Get(Stats type) => _definitions[type];
}
