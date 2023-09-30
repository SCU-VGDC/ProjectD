using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyswarm_Enemy_Behavior : Base_Enemy
{
    public Flyswarm_Pursue_State pursue_state;

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
 * In this state, the Flyswarm should actively chase the player, dealing damage on contact.
 **/
public class Flyswarm_Pursue_State : AI_State
{
    public override void Init(Base_Enemy context)
    {

    }

    public override void Action(Base_Enemy context)
    {

    }
}
