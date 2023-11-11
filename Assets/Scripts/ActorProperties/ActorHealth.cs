using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHealth : MonoBehaviour
{
    public int currentHealth;
    
    public int maxHealth;

    public Action DeathFunction;

    public float vulnerabilityCooldown = 1f; // cooldown in seconds between taking damage
    private bool isVulnerable = true;
    private float timeSinceLastHit;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void ApplyDamage(int damageAmount)
    {
        if (isVulnerable) { // damage cooldown must be inactive
            currentHealth -= damageAmount;

            if (currentHealth <= 0)
            {
                DeathFunction();
            }
            isVulnerable = false;
            timeSinceLastHit = 0f;
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
    }

    private void Update()
    {
        if (!isVulnerable) // counts damage cooldown
        {
            timeSinceLastHit += Time.deltaTime;
            if (timeSinceLastHit >= vulnerabilityCooldown)
            {
                isVulnerable = true;
            }
        }
    }
}
