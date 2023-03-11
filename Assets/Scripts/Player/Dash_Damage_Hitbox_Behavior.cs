using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash_Damage_Hitbox_Behavior : MonoBehaviour
{
    //dash damage
    [SerializeField] public int dash_damage;

    //enemy health
    public Enemy_Health enemy;

    void OnTriggerEnter2D(Collider2D hit_info)
    {
        if(hit_info.gameObject.layer == 9) //Note: enemy layer is 9
        {
            enemy = hit_info.GetComponent<Enemy_Health>();
            if(enemy != null)
            {
                enemy.Take_Damage(dash_damage);
            }
        }
    }
}
