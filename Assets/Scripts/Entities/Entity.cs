using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int startingHealth;
    public int currentHealth;

    public float speed;
    public float acceleration;
    public float deceleration;

    public Vector2 velocity;
    public Vector2 moveInput;
    public Vector2 lookDir;

    public int damageBonus;

    public virtual void Start() 
    {
        currentHealth = startingHealth;
    }

    public virtual void Damage(int amount, GameObject dealer)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Kill();

        GameEvents.current.DamageDealt(amount, dealer);
    }

    public virtual void Kill(){}
}
