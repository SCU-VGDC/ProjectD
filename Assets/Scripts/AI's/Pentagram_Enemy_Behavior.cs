using UnityEngine;

// Should always be chasing player,
// this will need to be manually enabled/disabled by another script using MonoBehaviour.enabled
public class Pentagram_Enemy_Behavior : Base_Enemy
{
    public Pentagram_Pursue_State pursue_state;
    public float acceleration = .3f;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        seeker.enabled = false;
        mover.enabled = false;

        //Set starting state here.
        pursue_state = new Pentagram_Pursue_State
        {
            acceleration = acceleration
        };
        current_state = pursue_state;
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
 * In this state, the Pentagram should actively chase the player, dealing damage on contact.
 * Pentagrams should ignore all collision when chasing, meaning it can go through walls.
 **/
public class Pentagram_Pursue_State : AI_State
{
    public float acceleration;
    private Transform playerTransform;
    private Vector3 velocity = Vector3.zero;

    public override void Init(Base_Enemy context)
    {
        playerTransform = GameManager.inst.player.transform;
    }

    public override void Action(Base_Enemy context)
    {
        Vector3 goalDir = playerTransform.position - context.transform.position;
        Vector3 accel = goalDir.normalized * (acceleration * Time.deltaTime);
        velocity += accel;
        float speed = Mathf.Min(velocity.magnitude, context.speed * Time.deltaTime);
        velocity = velocity.normalized * speed;
        context.transform.position += velocity;
    }
}
