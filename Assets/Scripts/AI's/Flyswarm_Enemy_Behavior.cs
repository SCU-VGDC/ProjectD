using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyswarm_Enemy_Behavior : Base_Enemy
{
    [Header("Properties")]
    public bool completed_path;
    public Vector3 nextPos;

    [Header("States")]
    public SimpleChaseState pursue_state;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        pursue_state = new SimpleChaseState(GameManager.inst.player.transform, speed);

        // Flyswarm only has pursue state, so it will never change from this
        current_state = pursue_state;
        current_state.Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        completed_path = seeker.GetCurrentPath() == null || mover.reachedEndOfPath;

        current_state.Action(this);

        Quaternion nextRot;
        mover.MovementUpdate(Time.deltaTime, out nextPos, out nextRot);
        mover.FinalizeMovement(nextPos, nextRot);
    }
}

/**
 * In this state, the Flyswarm should actively chase the player, dealing damage on contact.
 **/
[System.Serializable]
public class Flyswarm_Pursue_State : AI_State
{
    private Transform player_transform;

    public override void Init(Base_Enemy context)
    {
        player_transform = GameManager.inst.player.transform;
    }

    public override void Action(Base_Enemy context)
    {
        Flyswarm_Enemy_Behavior proper_context = (Flyswarm_Enemy_Behavior)context;
        Pursue(proper_context);

        //When attacking, face direction the player
        context.transform.localScale = (player_transform.position.x - proper_context.transform.position.x > 0) ? new Vector3(-1, 1, 1) : Vector3.one;
        Debug.Log("B");
    }

    public void Pursue(Flyswarm_Enemy_Behavior context)
    {
        //Try to reach the player.
        //Code lifted from https://stackoverflow.com/questions/300871/best-way-to-find-a-point-on-a-circle-closest-to-a-given-point
        //float vX = context.transform.position.x - player_transform.position.x;
        //float vY = context.transform.position.y - player_transform.position.y;
        //float magV = Mathf.Sqrt(vX * vX + vY * vY);
        float target_x = player_transform.position.x;
        float target_y = player_transform.position.y;

        Vector2 target_pos = new Vector2(target_x, target_y);

        //Actually build and send the path to the enemy!
        context.seeker.StartPath(context.transform.position, target_pos);
    }
}