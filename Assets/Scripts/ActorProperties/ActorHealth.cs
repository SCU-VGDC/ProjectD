using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHealth : MonoBehaviour
{
    public int currentHealth;
    
    public int maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void ApplyDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            EventManager.singleton.AddEvent(new actorDiedmsg(gameObject));
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
    }

    public void Die()
    {
        GetComponent<Base_Enemy>().Disable();
        Destroy(gameObject);
    }
}
