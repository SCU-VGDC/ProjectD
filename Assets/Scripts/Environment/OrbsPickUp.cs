using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;
using UnityEngine.Rendering.Universal;

public class OrbsPickUp : Interactable
{

    public Light2D light;
    public float time_orb_gone = 0f;
    public float orb_cool_down = 5f; // 5 seconds
    public bool can_see_orb = false;

    [SerializeField]
    private bool this_is_dash_through_orb = true;

    [Header("Game Manager")]
    private PersistentData pd;
    private PlayerMov_FSM player_movement;

    [Header("Graphics")]
    public Sprite reg_orb_sprite;
    public Sprite dash_orb_sprite;


    //private orb = new gameObject(typeof(SphereCollider));

    void Start()
    {
        pd = PersistentDataManager.inst.persistentData;

        player_movement = GameManager.inst.playerMovement;
        light = gameObject.GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        GetComponent<SpriteRenderer>().sprite = reg_orb_sprite;
        if (this_is_dash_through_orb)
        {
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

    void Update()
    {
        if (!can_see_orb)
        {
            if (time_orb_gone >= orb_cool_down)
            {
                can_see_orb = true;
                orb_visibility();
            }
            time_orb_gone += Time.deltaTime;
        }
    }

    public override void Activation()
    {
        //if the player is dashing and goes through, it'll disappear for 5s

        if (this_is_dash_through_orb && (player_movement.currentState != "Dash"))
        {
            return;
        }

        EventManager.singleton.AddEvent(new OrbPickUpmsg(gameObject, this_is_dash_through_orb));

        can_see_orb = false;
        orb_visibility();
        time_orb_gone = 0f;
    }


    //andrew's feedback
    /*
        - Don't wall slide until you start moving downwards
        - 
    */
}
