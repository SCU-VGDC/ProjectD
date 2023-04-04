using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

namespace Player_Movement_Namespace
{
    public class Dash_Through_Orb_Behavior: MonoBehaviour
    {
        public GameObject player;
        public Player_Movement player_movement;
        public float time_orb_gone = 0f; 
        public float orb_cool_down = 5f; // 5 seconds
        public bool see = false;

        [Header("Game Manager")]
        private PersistentData pd;

        // Start is called before the first frame update
        void Start()
        {
            pd = GameObject.Find("Persistent Data Manager").GetComponent<PersistentDataManager>().persistentData;

            player = GameObject.FindWithTag("Player");
            player_movement = player.GetComponent<Player_Movement>();
        }

        //changes orb status
        void orb_visibility()
        {
            GetComponent<Collider2D>().enabled = see;
            GetComponent<SpriteRenderer>().enabled = see;
        }

        void Update(){
            //controls timer for re-spawn
            if(!see)
            {
                if(time_orb_gone >= orb_cool_down)
                {
                    see = true;
                    orb_visibility();
                }
                time_orb_gone += Time.deltaTime;
            }
        }

        void OnTriggerEnter2D(Collider2D hit_info)
        {
            //if the player is dashing and goes through, it'll disappear for 5s
            if(hit_info.gameObject.layer == 7)
            {
                if(player_movement.is_dashing)
                {
                    // add one if you aren't at max
                    if(pd.PlayerNumDashes < player_movement.maximum_dashes) 
                    {
                        pd.AddPlayerNumDashes(1);
                    }

                    see = false;
                    orb_visibility();
                    time_orb_gone = 0f;
                }
                
            }
        }
    }
}