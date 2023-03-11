using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int health;
    public float damage_flash_time;

    [SerializeField] public SpriteRenderer sr;

    public Color normal_color;
    public Color damaged_color;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        normal_color = sr.color; //added to copy the actual color into var
    }

    public void Take_Damage(int damage)
    {
        health -= damage;
        
        StartCoroutine(wait_time());

        if(health <= 0)
        {
            Die();
        }
    }

    IEnumerator wait_time()
    {
        sr.color = damaged_color;
        yield return new WaitForSeconds(damage_flash_time);
        sr.color = normal_color;
    }

    public void Die()
    {
        if(transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        Destroy(gameObject);
    }
}
