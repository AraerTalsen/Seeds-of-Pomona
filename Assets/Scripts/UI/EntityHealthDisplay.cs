using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityHealthDisplay : MonoBehaviour
{
    [SerializeField] private Slider health;
    private EntityStats stats;

    private void Start()
    {
        stats = GetComponent<EntityStats>();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        Debug.Log($"Current: {stats.CurrentHealth}, Max: {stats.MaxHealth}, Solution: {1.0f - stats.CurrentHealth / (float)stats.MaxHealth}");
        health.value = 1.0f - stats.CurrentHealth / (float)stats.MaxHealth;
    }
}
