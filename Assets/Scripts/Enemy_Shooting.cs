using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy_Shooting : MonoBehaviour
{
    public float shooting_range;
    public float fire_rate;
    private float next_fire_time;

    public GameObject bullet_prefab;
    public Transform fire_point;
    private Transform player_transform;
    private AIPath AIPath_component;

    // Start is called before the first frame update
    void Start()
    {
        player_transform = GameObject.FindWithTag("Player").transform;
        AIPath_component = GetComponent<AIPath>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance_to_player = Vector2.Distance(player_transform.position, transform.position);
        //Debug.Log("distance is " + distance_to_player.ToString());

        //if the enemy's distance to the player is less then or equal to the shooting range, then stop moving and spawn a bullet
        if(distance_to_player <= shooting_range)
        {
            AIPath_component.enabled = false;

            if(next_fire_time < Time.time)
            {
                Instantiate(bullet_prefab, fire_point.position, fire_point.rotation);
                next_fire_time = Time.time + fire_rate;
            }
        }
        else
        {
            AIPath_component.enabled = true;
        }
    }

    void OnDrawGizmoSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shooting_range);
    }
}
