using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash_Orb_Behavior: MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    //make orb dissapear when you touch it?
    void OnTriggerEnter2D(Collider2D hit_info)
    {
        //if the enemy bullet hits a platform, the bounds, destroy itself
        if(hit_info.gameObject.layer == 7)
        {
            Debug.Log("touched orb!");
            Destroy(gameObject);
        }
    }
}
