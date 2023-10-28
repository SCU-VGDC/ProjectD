using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHealth : MonoBehaviour
{
    public int currentHealth;
    
    public int maxHealth;

    public void ApplyDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        
    }
}
