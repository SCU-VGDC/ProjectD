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

    [SerializeField] public GameObject blood_drop_prefab;

    void Start()
    {

        if (sr == null)
        {
            try
            {
                sr = GetComponent<SpriteRenderer>();
            }
            catch
            {
                sr = GetComponentInChildren<SpriteRenderer>();
            }
        }
        
        normal_color = sr.color; //added to copy the actual color into var
    }

    public void Take_Damage(int damage)
    {
        health -= damage;

        for(int i = 0; i < 5; i++)
        {
            Debug.Log("Blood!");
            Instantiate(blood_drop_prefab, gameObject.transform.position, Quaternion.identity); 
        }
        
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
            if (transform.parent.parent != null)
            {
                if(transform.parent.parent.GetComponent<RoomManager>())
                {
                    transform.parent.parent.GetComponent<RoomManager>().enemies.Remove(this);
                    transform.parent.parent.GetComponent<RoomManager>().amountOfEnemies--;
                }
            }
            Destroy(transform.parent.gameObject);
        }
        Destroy(gameObject);
    }
}
