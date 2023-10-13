using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baphomet_Enemy_Behavior : Base_Enemy
{
    public Baphomet_Idle_State idle_state;
    public Baphomet_Attack_State attack_state;


    // Start is called before the first frame update
    void Start()
    {
        base.Init();

        //Set starting state here.
        //Run starting state's Init().
    }

    // Update is called once per frame
    void Update()
    {
        current_state.Action(this);
    }
}

/**
 * In this state, Baphomet should stand still, dealing damage on contact.
 **/
public class Baphomet_Idle_State : AI_State
{
    public override void Init(Base_Enemy context)
    {

    }

    public override void Action(Base_Enemy context)
    {
        //Make sure that if Baphomet's Pentagram hasn't spawned yet it transitions into its Attack state to spawn it!
    }
}

/**
 * In this state, Baphomet should spawn a new Pentagram.
 * 
 * TODO: It might be worth creating a new Pool in GameManager to hold these, as Instantiating repeatedly can be costly.
 * Don't do this immedietally unless you're confident however, as it's a super premature optimization at the moment.
 **/
public class Baphomet_Attack_State : AI_State
{
    public override void Init(Base_Enemy context)
    {

    }

    public override void Action(Base_Enemy context)
    {

    }
}
