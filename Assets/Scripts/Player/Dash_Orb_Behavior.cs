using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

namespace Player_Movement_Namespace
{
    public class Dash_Orb_Behavior: MonoBehaviour
    {
        public GameObject player;
        //public Player_Movement player_movement; legacy
        public UnityEngine.Rendering.Universal.Light2D light;
        public float time_orb_gone = 0f; 
        public float orb_cool_down = 5f; // 5 seconds
        public bool see = false;

        [Header("Game Manager")]
        private PersistentData pd;
        private SpriteRenderer sr;
        private Collider2D collider;

        // Start is called before the first frame update
        void Start()
        {
            pd = PersistentDataManager.inst.persistentData;

            player = GameObject.FindWithTag("Player");
          //  player_movement = player.GetComponent<Player_Movement>(); legacy
            light = gameObject.GetComponent<UnityEngine.Rendering.Universal.Light2D>();

            GetComponent<Collider2D>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;

            collider = gameObject.GetComponent<Collider2D>();
            sr = gameObject.GetComponent<SpriteRenderer>();
        }

        void orb_visibility()
        {
            collider.enabled = see;
            sr.enabled = see;
            light.enabled = see;
        }

        void Update()
        {
            //controls timer for respawn
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

        //orb disappears after touch for 5s
        void OnTriggerEnter2D(Collider2D hit_info)
        {
            //tells if player (on layer 7) is in orb
            if(hit_info.gameObject.layer == 7)
            {
                //Debug.Log("touched orb!");
                // Debug.Log(player_movement.current_dashes);
                // Debug.Log(player_movement.maximum_dashes);
                
                if(GameManager.inst.playerMovement.currentDashes < pd.PlayerMaximumDashes)
                {
                    GameManager.inst.playerMovement.currentDashes++;
                }
                else
                {
                    // Debug.Log("already at max dashes!");
                }

                see = false;
                orb_visibility();
                time_orb_gone = 0f;
            }
        }
    }
}