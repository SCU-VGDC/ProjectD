using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHealth : MonoBehaviour
{
    bool isVulnerable;
    public bool isInvincible = false;
    float timeSinceLastHit;
    public float vulnerabilityCooldown;
    public int currentHealth;
    public bool died;
    
    public int maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void ApplyDamage(int damageAmount)
    {
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (isVulnerable && !isInvincible)
        {// damage cooldown must be inactive
            currentHealth -= damageAmount;
            isVulnerable = false;
        }

        if (currentHealth <= 0 && !died)
        {
            died = true;
            if(gameObject.tag == "Player")
            {
                EventManager.singleton.AddEvent(new playerDiedmsg());
            }
            else
            {
                EventManager.singleton.AddEvent(new actorDiedmsg(gameObject));
            }
            isVulnerable = false;
            timeSinceLastHit = 0f;
        }
    }

    private void Update()
    {
        if (!isVulnerable) // counts damage cooldown
        {
            timeSinceLastHit += Time.deltaTime;
            if (timeSinceLastHit >= vulnerabilityCooldown)
            {
                isVulnerable = true;
                timeSinceLastHit = 0;
            }
        }
    }
}
