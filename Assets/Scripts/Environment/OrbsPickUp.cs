using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

public class OrbsPickUp : MonoBehaviour
{

    public GameObject player;
    public UnityEngine.Rendering.Universal.Light2D light;
    public float time_orb_gone = 0f; 
    public float orb_cool_down = 5f; // 5 seconds
    public bool can_see_orb = false;

    [SerializeField]
    private bool this_is_dash_through_orb = true;

    [Header("Game Manager")]
    private PersistentData pd;
    private SpriteRenderer sr;
    private PlayerMov_FSM player_movement;

    [Header("Graphics")]
    public Sprite reg_orb_sprite;
    public Sprite dash_orb_sprite;


    //private orb = new gameObject(typeof(SphereCollider));

    // Start is called before the first frame update
    void Start()
    {
        pd = PersistentDataManager.inst.persistentData;

        player = GameObject.FindWithTag("Player");
        player_movement = GameManager.inst.playerMovement;
        light = gameObject.GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        GetComponent<SpriteRenderer>().sprite = reg_orb_sprite;
        if (this_is_dash_through_orb){
            GetComponent<SpriteRenderer>().sprite = dash_orb_sprite;
        }
    }

    //changes orb status
    void orb_visibility()
    {
        GetComponent<CircleCollider2D>().enabled = can_see_orb;
        GetComponent<SpriteRenderer>().enabled = can_see_orb;
        light.enabled = can_see_orb;
    }

    // Update is called once per frame
    void Update()
    {
        if(!can_see_orb)
            {
                if(time_orb_gone >= orb_cool_down)
                {
                    can_see_orb = true;
                    orb_visibility();
                }
                time_orb_gone += Time.deltaTime;
                Debug.Log(Time.deltaTime);
            }
    }

    void OnTriggerEnter2D(Collider2D hit_info)
        {
            //if the player is dashing and goes through, it'll disappear for 5s
            if(hit_info.gameObject.layer == 7)
            {
                if((player_movement.currentState=="Dash") && this_is_dash_through_orb)
                {
                    // add one if you aren't at max
                    if(player_movement.currentDashes < pd.PlayerMaximumDashes) 
                    {
                        player_movement.currentDashes++;
                    }

                    can_see_orb = false;
                    orb_visibility();
                    time_orb_gone = 0f;
                }
                
                if(!this_is_dash_through_orb){
                    if(player_movement.currentDashes < pd.PlayerMaximumDashes)
                    {
                        player_movement.currentDashes++;
                    }
                    else
                    {
                        // Debug.Log("already at max dashes!");
                    }

                    can_see_orb = false;
                    orb_visibility();
                    time_orb_gone = 0f;
                }
            }
        }
        //andrew's feedback
        /*
            - Don't wall slide until you start moving downwards
            - 
        */
}
