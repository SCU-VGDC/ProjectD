using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood_Behavior : MonoBehaviour
{
    //GameObject Vars
    Rigidbody2D rb;
    TrailRenderer tr;

    //Number vars
    [SerializeField] public float thrust_upper;
    [SerializeField] public float thrust_lower;
    private float thrust;
    private float trajectory_x;
    public float trajectory_range;

    // Start is called before the first frame update
    void Start()
    {
        //Variable setup
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();

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

    }
}
