using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank_Enemy_Behavior : Base_Enemy
{
    public Tank_Idle_State idle_state;
    public Tank_Attack_State attack_state;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();

        //Set starting state here.
        current_state = idle_state;
        //Run starting state's Init().
        current_state.Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        current_state.Action(this);
    }
}

/**
 * In this state, the Tank should simply stand still with an active hitbox
 * to deal damage to the Player on contact.
 **/
public class Tank_Idle_State : AI_State
{
    public override void Init(Base_Enemy context)
    {

    }

    public override void Action(Base_Enemy context)
    {
        //If the Player is within range, transition to Attack.
    }
}

/**
 * In this state, the Tank should activate an attack animation, with a
 * hitbox that deals damage to the player. When the animation is complete, go back to Idle.
 **/
public class Tank_Attack_State : AI_State
{
    public override void Init(Base_Enemy context)
    {

    }

    public override void Action(Base_Enemy context)
    {

    }
}