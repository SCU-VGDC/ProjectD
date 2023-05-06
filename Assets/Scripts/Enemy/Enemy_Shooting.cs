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
    private AIDestinationSetter destination_setter;

    // Start is called before the first frame update
    void Start()
    {
        player_transform = GameObject.FindWithTag("Player").transform;
        AIPath_component = GetComponent<AIPath>();
        destination_setter = GetComponent<AIDestinationSetter>();

        destination_setter.target = player_transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(fire_point.position, player_transform.position - transform.position);

        RaycastHit2D hit = Physics2D.Raycast(fire_point.position, player_transform.position - transform.position, shooting_range, LayerMask.GetMask("Player", "Platforms"));
        if (next_fire_time < Time.time && hit.collider != null && hit.collider.tag == "Player")
        {
            //TODO: Make bullet's fire angle based on angle between target and enemy
            Instantiate(bullet_prefab, fire_point.position, Quaternion.identity);
            next_fire_time = Time.time + fire_rate;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shooting_range / 2);
    }
}
