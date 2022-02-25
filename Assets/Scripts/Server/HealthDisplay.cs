using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health = null;
    [SerializeField] private Image healthBarImage = null;

    private void OnEnable()
    {
        health.onHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        health.onHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
