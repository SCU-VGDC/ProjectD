using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace
{
    public class Dash_Orb_Behavior: MonoBehaviour
    {
        public GameObject player;
        public Player_Movement player_movement;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindWithTag("Player");
            player_movement = player.GetComponent<Player_Movement>();
        }

        //make orb dissapear when you touch it?
        void OnTriggerEnter2D(Collider2D hit_info)
        {
            //if the enemy bullet hits a platform, the bounds, destroy itself
            if(hit_info.gameObject.layer == 7)
            {
                Debug.Log("touched orb!");
                Debug.Log(player_movement.current_dashes);
                Debug.Log(player_movement.maximum_dashes);

                if(player_movement.current_dashes<player_movement.maximum_dashes)
                {
                    player_movement.current_dashes+=100;
                }
                else
                {
                    Debug.Log("already at max dashes!");
                }

                Destroy(gameObject);
            }
        }
    }
}