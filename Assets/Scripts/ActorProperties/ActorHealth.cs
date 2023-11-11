using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHealth : MonoBehaviour
{
    bool isVulnerable;
    float timeSinceLastHit;
    public float vulnerabilityCooldown;
    public int currentHealth;
    
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

        if (isVulnerable)
        {// damage cooldown must be inactive
            currentHealth -= damageAmount;
            isVulnerable = false;
        }

        if (currentHealth <= 0)
        {
            EventManager.singleton.AddEvent(new actorDiedmsg(gameObject));
            isVulnerable = false;
            timeSinceLastHit = 0f;
        }
    }

    public void Die()
    {
        GetComponent<Base_Enemy>().Disable();
        Destroy(gameObject);
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
