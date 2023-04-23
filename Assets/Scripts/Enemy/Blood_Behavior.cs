using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

public class Blood_Behavior : MonoBehaviour
{
    //GameObject Vars
    private Rigidbody2D rb;
    private TrailRenderer tr;
    private PersistentData pd;

    //Number vars
    [SerializeField] public float thrust_upper;
    [SerializeField] public float thrust_lower;
    private float thrust;
    private float trajectory_x;
    [SerializeField] public float trajectory_range;

    //Attraction to player vars
    [SerializeField] private float detection_range;
    private Transform player_transform;
    private Vector3 dir_to_player;
    private float dst_to_player;
    private float force_magnitude;
    private Vector3 force;
    [SerializeField] private float gravitational_constamt;
    [SerializeField] private float fake_player_mass;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        //Variable setup
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        player_transform = GameObject.FindWithTag("Player").transform;
        pd = GameObject.Find("Persistent Data Manager").GetComponent<PersistentDataManager>().persistentData;

        //calculate random thrust and trajectory
        thrust = Random.Range(thrust_lower, thrust_upper);
        trajectory_x = Random.Range(-trajectory_range, trajectory_range);

        //apply these random values to blood drop
        rb.AddForce(new Vector3(trajectory_x, 2, 0) * thrust);

        //turn on trail renderer
        tr.emitting = true;

        //make sure blood and player ignore collision
        //NOTE: Yeah I know that getting the player's collider via the player's transform is weird but it saves an unnecessary search 
        //Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), player_transform.gameObject.GetComponent<Collider2D>());
    }

    //OnCollisionEnter2D called when blood drop collides with something
    void OnCollisionEnter2D(Collision2D collider)
    {
        if(collider.gameObject.layer == 16)
        {
            //heal player
            pd.AddPlayerHealth(1);
            Debug.Log("Heal!");
        }

        //destroy self
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //calculate direction of player
        dir_to_player = player_transform.position - transform.position;

        //calculate distance to player
        dst_to_player = dir_to_player.sqrMagnitude;

        //calculate force magnitude
        force_magnitude = (rb.mass * fake_player_mass) / Mathf.Pow(dst_to_player, 2);

        //calculate force
        force = dir_to_player.normalized * force_magnitude;

        //apply force to blood drop rigidbody
        rb.AddForce(force);
    }
}
