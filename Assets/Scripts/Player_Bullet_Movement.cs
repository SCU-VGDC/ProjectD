using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet_Movement : MonoBehaviour
{
    //bullet damage and speed
    public int player_bullet_damage;
    public float player_bullet_speed;
    
    //Gameobject variables
    [SerializeField] public Rigidbody2D rb;
    private Camera main_camera;

    //mouse position
    private Vector3 mouse_pos;

    //enemy health
    public Enemy_Health enemy;

    void Start()
    {
        //gameobject assignment
        main_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mouse_pos = main_camera.ScreenToWorldPoint(Input.mousePosition);
        //shoot direction calculation
        Vector3 direction = mouse_pos - transform.position;
        Vector3 rotation = transform.position - mouse_pos;
        rb.velocity = new Vector3(direction.x, direction.y).normalized * player_bullet_speed;
        //bullet rotation calculation
        float bullet_rotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, bullet_rotation + 90);
    }

    void OnTriggerEnter2D(Collider2D hit_info)
    {
        //Debug.Log("the bullet hit" + hit_info.name);

        if(hit_info.gameObject.layer == 9) //Note: enemy layer is 9
        {
            enemy = hit_info.GetComponent<Enemy_Health>();
            if(enemy != null)
            {
                enemy.Take_Damage(player_bullet_damage);
            }

            Destroy(gameObject);
        }
        else if(hit_info.gameObject.layer == 6 || hit_info.gameObject.layer == 10) //Note: plats layer is 6 and bounds layer is 10
        {
            Destroy(gameObject);
        }
    }
}
