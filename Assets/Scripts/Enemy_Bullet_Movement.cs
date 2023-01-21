using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet_Movement : MonoBehaviour
{
    public float bullet_speed;

    public GameObject target;
    private Rigidbody2D bullet_rb;

    // Start is called before the first frame update
    void Start()
    {
        bullet_rb = GetComponent<Rigidbody2D>();

        target = GameObject.FindWithTag("Player");
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector2 move_dir = (target.transform.position - transform.position).normalized * bullet_speed;
        
        bullet_rb.velocity = new Vector2(move_dir.x, move_dir.y);
    }

    void OnTriggerEnter2D(Collider2D hit_info)
    {
        //if the enemy bullet hits a platform, the bounds, destroy itself
        if(hit_info.gameObject.layer == 6 || hit_info.gameObject.layer == 10)
        {
            Destroy(gameObject);
        }
    }
}
