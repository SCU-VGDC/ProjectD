using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHealth : MonoBehaviour
{
    public int currentHealth;
    
    public int maxHealth;

    public Action DeathFunction;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void ApplyDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            DeathFunction();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
    }
}
