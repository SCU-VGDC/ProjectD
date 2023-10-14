using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHealth : MonoBehaviour
{
    public int currentHealth;
    
    public int maxHealth;

    public float invulnerability_time = 0.0f;

    public void ApplyDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
    }

    public void GiveInvulnerability()
    {
        invulnerability_time = 2.0f;
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
    }

    void Update() {
        if (invulnerability_time > 0) {
            invulnerability_time -= Time.deltaTime;
        }
    }
}
