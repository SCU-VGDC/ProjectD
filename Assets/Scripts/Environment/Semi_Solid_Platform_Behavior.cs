using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Semi_Solid_Platform_Behavior: MonoBehaviour
{
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
            effector.surfaceArc = 0f;
        }
        else
        {
            //flip offset up so it faces upwards
            effector.surfaceArc = 132.64f;
        }
    }
}
