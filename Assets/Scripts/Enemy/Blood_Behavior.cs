using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood_Behavior : MonoBehaviour
{
    //GameObject Vars
    private Rigidbody2D rb;
    private TrailRenderer tr;
    private Transform player_transform;

    //Number vars
    [SerializeField] public float thrust_upper;
    [SerializeField] public float thrust_lower;
    [SerializeField] private float detection_range;
    private float thrust;
    private float trajectory_x;
    [SerializeField] public float trajectory_range;
    private Vector3 dir_to_player;
    [SerializeField]

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        //Variable setup
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        player_transform = GameObject.FindWithTag("Player").transform;

        //calculate random thrust and trajectory
        thrust = Random.Range(thrust_lower, thrust_upper);
        trajectory_x = Random.Range(-trajectory_range, trajectory_range);

        //apply these random values to blood drop
        rb.AddForce(new Vector3(trajectory_x, 2, 0) * thrust);

        //turn on trail renderer
        tr.emitting = true;
    }

    //OnCollisionEnter2D called when blood drop collides with something
    void OnCollisionEnter2D(Collision2D collider)
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //calculate direction of player
        dir_to_player = player_transform.position - transform.position;

        //cast a ray from blood to player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir_to_player, detection_range, LayerMask.GetMask("Player", "Platforms"));

        //check if raycast hit
        if(hit.collider != null && hit.collider.tag == "Player")
        {
            //apply force
            rb.AddForce(dir_to_player);
        }
    }
}
