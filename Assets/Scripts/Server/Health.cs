using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 20;
    
    [SyncVar] private int currentHealth;

    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged onHealthChanged;

    [ClientRpc]
    private void RpcOnHealthChanged(int currentHealth, int maxHealth)
    {
        this.onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    [ServerCallback]
    private void SetHealth(int value)
    {
        currentHealth = value;
        this.onHealthChanged?.Invoke(currentHealth, maxHealth);
        RpcOnHealthChanged(currentHealth, maxHealth);
    }

    public override void OnStartServer() => SetHealth(maxHealth);

    [ServerCallback] 
    public void DealDamage(int value)
    {
        if(!isServer)
            return;

        SetHealth(Mathf.Max(currentHealth - value, 0));

        if (currentHealth == 0)
        {
            if(gameObject.TryGetComponent<Entity>(out var entity))
                entity.Kill();

            // Reset Health
            SetHealth(maxHealth);
        }
    }

    [ServerCallback]
    public void ResetHealth()
    {
        SetHealth(maxHealth);
    }
}
