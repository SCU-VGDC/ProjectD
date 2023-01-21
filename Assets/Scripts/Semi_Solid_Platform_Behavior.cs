using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Semi_Solid_Platform_Behavior: MonoBehaviour
{
    //private float pass_through_time = 0.3f;
    //private float pass_through_time_counter;
    private PlatformEffector2D effector;

    // Start is called before the first frame update
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();    
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Crouch"))
        {
            //flip offset down so it faces downwards
            //effector.rotationalOffset = 180f;
            effector.surfaceArc = 0f;
            //pass_through_time_counter
            //pass_through_time_counter = pass_through_time;
        }
        else
        {
            effector.surfaceArc = 132.64f;
        }
        /*
        else
        {
            //count down pass_through_time_counter
            pass_through_time_counter -= Time.deltaTime;
        }
        */

        /*
        if(Input.GetButtonDown("Jump") || pass_through_time_counter < 0)
        {
            //reset offset so it faces upwards
            //effector.rotationalOffset = 0f;
            effector.surfaceArc = 132.64f;
            //reset pass_through_time_counter
            pass_through_time_counter = 0f;
        }
        */
    }
}
