using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ActorHealth))]
public class FallenAngel_Enemy_Behavior : Base_Enemy
{
    public CircleCollider2D col;
    private ActorHealth health;
    public int repathRate = 60;

    [Header("Properties")]
    public float aggro_radius;
    public bool completed_path;
    public Vector3 nextPos;
    public SpriteRenderer sr;

    [Header("States")]
    public FallenAngel_Idle_State idle_state;
    public FallenAngel_Aggro_State aggro_state;

    void Start()
    {
        col = GetComponent<CircleCollider2D>();
        health = GetComponent<ActorHealth>();

        //Initializes some important variables contained in Base_Enemy
        base.Init();

        //Start out wandering
        current_state = idle_state;
        current_state.Init(this);
    }

    void Update()
    {
        // freeze and disable collider when dead
        if (health.died)
        {
            col.enabled = false;
            mover.enabled = false;
            return;
        }

        Debug.Log("Fallen Angel Current state is: " + current_state.ToString());
        completed_path = seeker.GetCurrentPath() == null || mover.reachedEndOfPath;

        current_state.Action(this);
       
        Quaternion nextRot;
        mover.MovementUpdate(Time.deltaTime, out nextPos, out nextRot);
        mover.FinalizeMovement(nextPos, nextRot);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggro_state.attack_radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggro_radius);

        if (!Application.isPlaying) return;

        current_state.OnDrawGizmos(this);
    }
}

[System.Serializable]
public class FallenAngel_Idle_State : AI_State
{

    private Transform player_transform;


    public override void Init(Base_Enemy context)
    {
        player_transform = GameManager.inst.player.transform;
    }

    public override void Action(Base_Enemy context)
    {
        //Gross casting to get all the properties we need. Maybe a better solution for this?
        FallenAngel_Enemy_Behavior proper_context = (FallenAngel_Enemy_Behavior)context;

        if (Vector2.Distance(player_transform.position, proper_context.transform.position) < proper_context.aggro_radius)
        {
            proper_context.Transition(proper_context.aggro_state);
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(context.gameObject, true));
        }
    }
}


/* TODO:
 * This state is never used, just kept here in case we ever want Belzebub to wander in the future.
 */
[System.Serializable]
public class FallenAngel_Wander_State : AI_State
{
    public float wander_radius = 5;
    public float wander_delay = 3;

    private float next_wander;

    private Transform player_transform;


    public override void Init(Base_Enemy context)
    {
        player_transform = GameManager.inst.player.transform;
    }

    public override void Action(Base_Enemy context)
    {
        //Gross casting to get all the properties we need. Maybe a better solution for this?
        FallenAngel_Enemy_Behavior proper_context = (FallenAngel_Enemy_Behavior)context;

        if (proper_context.completed_path && Time.time > next_wander)
        {
            Wander(proper_context);
        }

        //When wandering, face direction that it's moving
        if (!proper_context.completed_path)
            proper_context.sr.flipX = (proper_context.nextPos.x - proper_context.transform.position.x < 0);

        if (Vector2.Distance(player_transform.position, proper_context.transform.position) < proper_context.aggro_radius)
        {
            proper_context.Transition(proper_context.aggro_state);
        }
    }

    public void Wander(FallenAngel_Enemy_Behavior context)
    {
        /*
         * First, get a random point nearby to wander do.
         * Since Beelzebub can fly, we just have to keep him outside of collision.
         */
        Vector2 target_pos = (Vector2)context.transform.position + (Random.insideUnitCircle * wander_radius);

        //Actually build and send the path to the enemy!
        context.seeker.StartPath(context.transform.position, target_pos);

        //BUG: This doesn't belong here! Move it somewhere else or just make the wander_delay really long
        next_wander = Time.time + wander_delay;
    }
}

[System.Serializable]
public class FallenAngel_Aggro_State : AI_State
{
    /* TODO:
     * This needs a timer that begins when the enemy loses sight of the player, afterwhich the mob will drop aggro and return to Wandering
     */

    [Header("Attack Properties")]
    public float attack_radius = 10f;
    public int melee_damage = 0;

    private Transform player_transform;

    public override void Init(Base_Enemy context)
    {
        player_transform = GameManager.inst.player.transform;
    }

    public override void Action(Base_Enemy context)
    {
        FallenAngel_Enemy_Behavior proper_context = (FallenAngel_Enemy_Behavior)context;

        if (proper_context.completed_path && Time.frameCount % proper_context.repathRate == 0)
        {
            //if player is in attack range,...
            if (Vector2.Distance(player_transform.position, context.transform.position) < attack_radius)
            {
                //stop fallen angel from moving
                context.mover.canMove = false;

                //stop moving
                EventManager.singleton.AddEvent(new ChangedMOVstatemsg(context.gameObject, false));

                //Attack the player
                EventManager.singleton.AddEvent(new meleeDamagemsg(context.gameObject, player_transform, melee_damage));
            }
            //else player is out of attack range,...
            else
            {
                //let fallen angel move
                context.mover.canMove = true;

                //move towards player
                context.seeker.StartPath(context.transform.position, player_transform.position);
                EventManager.singleton.AddEvent(new ChangedMOVstatemsg(context.gameObject, true));
            }
        }

        //When attacking, face direction the player
        context.transform.localScale = (player_transform.position.x - proper_context.transform.position.x > 0) ? new Vector3(-1, 1, 1) : Vector3.one;
    }
}