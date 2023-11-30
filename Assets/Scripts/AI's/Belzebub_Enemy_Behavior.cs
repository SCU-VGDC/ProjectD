using UnityEngine;


[RequireComponent(typeof(ActorShooting))]
public class Belzebub_Enemy_Behavior : Base_Enemy
{
    public CircleCollider2D col;
    public int repathRate = 60;

    [Header("Properties")]
    public float aggro_radius;
    public bool completed_path;
    public Vector3 nextPos;
    public SpriteRenderer sr;

    [Header("States")]
    public Belzebub_Idle_State idle_state;
    public Belzebub_Aggro_State aggro_state;

    void Start()
    {
        col = GetComponent<CircleCollider2D>();
        //Initializes some important variables contained in Base_Enemy
        base.Init();

        //Start out wandering
        current_state = idle_state;
        current_state.Init(this);
    }

    void Update()
    {
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
public class Belzebub_Idle_State : AI_State
{

    private Transform player_transform;


    public override void Init(Base_Enemy context)
    {
        player_transform = GameManager.inst.player.transform;
    }

    public override void Action(Base_Enemy context)
    {
        //Gross casting to get all the properties we need. Maybe a better solution for this?
        Belzebub_Enemy_Behavior proper_context = (Belzebub_Enemy_Behavior)context;

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
public class Belzebub_Wander_State : AI_State
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
        Belzebub_Enemy_Behavior proper_context = (Belzebub_Enemy_Behavior)context;

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

    public void Wander(Belzebub_Enemy_Behavior context)
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
public class Belzebub_Aggro_State : AI_State
{
    /* TODO:
     * This needs a timer that begins when the enemy loses sight of the player, afterwhich the mob will drop aggro and return to Wandering
     */

    [Header("Attack Properties")]
    public float attack_radius = 10f;
    public float fire_delay = 2f;
    public float flyswarm_delay = 3.5f;
    public float next_fire_time;
    public GameObject flyswarm;
    public Transform fire_point;

    private Transform player_transform;

    public override void Init(Base_Enemy context)
    {
        player_transform = GameManager.inst.player.transform;
    }

    public override void Action(Base_Enemy context)
    {
        Belzebub_Enemy_Behavior proper_context = (Belzebub_Enemy_Behavior)context;

        if (proper_context.completed_path && Time.frameCount % proper_context.repathRate == 0)
        {
            if (Vector2.Distance(player_transform.position, context.transform.position) > proper_context.aggro_radius)
            {
                //TODO: Save last seen player location & velocity and search nearby!
                //For now, just wander again.
                //proper_context.Transition(proper_context.idle_state);
                Pursue(proper_context);
                EventManager.singleton.AddEvent(new ChangedMOVstatemsg(context.gameObject, false));
            }
            else
            {
                Hover(proper_context);
                RaycastHit2D hit = Physics2D.Raycast(fire_point.position, player_transform.position - fire_point.transform.position, attack_radius, LayerMask.GetMask("Player"));
                if (next_fire_time < Time.time)
                {
                    float randy = Random.Range(0f, 1f);
                    if (randy < 0.66f && hit.collider != null && hit.collider.tag == "Player")
                    {
                        EventManager.singleton.AddEvent(new shootmsg(proper_context.gameObject, player_transform));
                        next_fire_time = Time.time + fire_delay;
                    }
                    else
                    {
                        Object.Instantiate(flyswarm, fire_point.position, Quaternion.identity);
                        next_fire_time = Time.time + flyswarm_delay;
                    }
                }
            }
        }

        //When attacking, face direction the player
        context.transform.localScale = (player_transform.position.x - proper_context.transform.position.x > 0) ? new Vector3(-1, 1, 1) : Vector3.one;
    }

    public void Pursue(Belzebub_Enemy_Behavior context)
    {
        float pursueVariance = 2f;

        //Try to stay within a safe distance of the player.
        //Code lifted from https://stackoverflow.com/questions/300871/best-way-to-find-a-point-on-a-circle-closest-to-a-given-point
        float vX = context.transform.position.x - player_transform.position.x;
        float vY = context.transform.position.y - player_transform.position.y;
        float magV = Mathf.Sqrt(vX * vX + vY * vY);
        float target_x = player_transform.position.x + vX / magV * attack_radius;
        float target_y = player_transform.position.y + vY / magV * attack_radius;

        Vector2 target_pos = new Vector2(target_x, target_y) + (Random.insideUnitCircle * pursueVariance);

        while (Physics2D.OverlapCircle(target_pos, context.col.radius, LayerMask.NameToLayer("Platforms")) != null)
        {
            pursueVariance += 0.5f;
            target_pos = new Vector2(target_x, target_y) + (Random.insideUnitCircle * pursueVariance);
        }

        //Actually build and send the path to the enemy!
        context.seeker.StartPath(context.transform.position, target_pos);
    }

    public void Hover(Belzebub_Enemy_Behavior context)
    {
        float hoverVariance = 3f;

        Vector2 target_pos = new Vector2(context.transform.position.x, context.transform.position.y) + (Random.insideUnitCircle * hoverVariance);

        while (Physics2D.OverlapCircle(target_pos, context.col.radius, LayerMask.NameToLayer("Platforms")) != null)
        {
            target_pos = new Vector2(context.transform.position.x, context.transform.position.y) + (Random.insideUnitCircle * hoverVariance);
        }

        //Actually build and send the path to the enemy!
        context.seeker.StartPath(context.transform.position, target_pos);
    }

    public override void OnDrawGizmos(Base_Enemy context)
    {
        Vector3 targetPos = GameManager.inst.player.transform.position;
        targetPos.z = fire_point.position.z;
        Vector3 dir = targetPos - fire_point.position;

        Gizmos.color = Color.black;
        Gizmos.DrawRay(fire_point.position, dir);
    }
}