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


        if (isVulnerable && !isInvincible && damageAmount > 0)
        {// damage cooldown must be inactive
            currentHealth -= damageAmount;
            isVulnerable = false;
        }

        // if healing, ignore the vulnerability cooldown
        if (damageAmount < 0) 
        {
            // make sure healing doesnt go over max health
            if (currentHealth - damageAmount >= maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth -= damageAmount;
            }
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
                gameObject.GetComponent<Collider2D>().enabled = false;
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
