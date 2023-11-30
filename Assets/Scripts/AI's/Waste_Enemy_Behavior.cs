using System;
using UnityEngine;

public class Waste_Enemy_Behavior : Base_Enemy
{
    [Header("Waste Properties")]
    public Transform point1;
    public Transform point2;
    public Waste_Idle_Patrol idlePatrolState;
    public Waste_Chase wasteChaseState;
    private float leftX;
    private float rightX;
    private float speedMult = 1;

    public int Direction { get; private set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        Init();

        // Hide the endpoints in-game
        foreach (Renderer renderer in point1.GetComponents<Renderer>()) Destroy(renderer);
        foreach (Renderer renderer in point2.GetComponents<Renderer>()) Destroy(renderer);

        leftX = MathF.Min(point1.position.x, point2.position.x);
        rightX = MathF.Max(point1.position.x, point2.position.x);

        current_state = idlePatrolState;
        current_state.Init(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        current_state.Action(this);

        float rawX = transform.position.x + (speed * speedMult * Direction * Time.deltaTime);
        transform.position = new Vector2(Mathf.Clamp(rawX, leftX, rightX), transform.position.y);
    }

    public void SetDirection(int nextDir)
    {
        if (nextDir == Direction) return;

        if (nextDir == 0)
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(gameObject, false));
        else if (Direction == 0)
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(gameObject, true));

        SpriteRenderer spriteR = GetComponent<SpriteRenderer>();
        spriteR.flipX = nextDir != 0 ? nextDir == -1 : spriteR.flipX;
        Direction = nextDir;
    }

    [Serializable]
    public class Waste_Idle_Patrol : AI_State
    {
        private float walkTime = 0;
        private float walkTimeGoal = 0;
        public float minIdleWalkTime = 0.5f;
        public float maxIdleWalkTime = 4f;
        public float aggroRange = 5;
        public bool aggroOnlyInFront = false;

        public override void Init(Base_Enemy context)
        {
            walkTime = 0;
            walkTimeGoal = 0;
            ((Waste_Enemy_Behavior)context).speedMult = 1;
        }
        public override void Action(Base_Enemy baseContext)
        {
            Waste_Enemy_Behavior context = (Waste_Enemy_Behavior)baseContext;
            bool oobLeft = context.transform.position.x <= context.leftX;
            bool oobRight = context.transform.position.x >= context.rightX;
            walkTime += Time.deltaTime;

            switch (oobLeft, oobRight)
            {
                case (false, false) when walkTime >= walkTimeGoal:
                    context.SetDirection(UnityEngine.Random.Range(-1, 2));
                    walkTime = 0;
                    walkTimeGoal = UnityEngine.Random.Range(minIdleWalkTime, maxIdleWalkTime);
                    break;
                case (true, false):
                    walkTime = 0;
                    context.SetDirection(1);
                    break;
                case (false, true):
                    walkTime = 0;
                    context.SetDirection(-1);
                    break;
            }

            Vector2 playerPos = GameManager.inst.player.transform.position;
            int playerDir = Math.Sign(playerPos.x - context.transform.position.x);
            bool inRange = Vector2.Distance(playerPos, context.transform.position) <= aggroRange;
            bool inFront = playerDir == context.Direction;
            if (inRange && (!aggroOnlyInFront || inFront))
                context.Transition(context.wasteChaseState);
        }
    }

    [Serializable]
    public class Waste_Chase : AI_State
    {
        private float giveUpTimeCounter = 0;
        public float giveUpRange = 10;
        public float giveUpTime = 2;
        public float aggroSpeedMult = 1.4f;

        public override void Init(Base_Enemy context)
        {
            giveUpTimeCounter = 0;
            ((Waste_Enemy_Behavior)context).speedMult = aggroSpeedMult;
        }
        public override void Action(Base_Enemy baseContext)
        {
            Waste_Enemy_Behavior context = (Waste_Enemy_Behavior)baseContext;
            Transform playerTrans = GameManager.inst.player.transform;
            float xdiff = playerTrans.position.x - context.transform.position.x;
            int sign = Math.Sign(xdiff);
            float fromPlr = Math.Abs(xdiff);
            bool oobLeft = context.transform.position.x <= context.leftX;
            bool oobRight = context.transform.position.x >= context.rightX;

            switch (sign)
            {
                case -1 when !oobLeft && fromPlr > 0.8f:
                    context.SetDirection(-1);
                    break;
                case 1 when !oobRight && fromPlr > 0.8f:
                    context.SetDirection(1);
                    break;
                default:
                    context.SetDirection(0);
                    break;
            }

            bool plrOutOfRange = Vector2.Distance(playerTrans.position, context.transform.position) > giveUpRange;
            giveUpTimeCounter = plrOutOfRange ? giveUpTimeCounter + Time.deltaTime : 0;
            if (giveUpTimeCounter > giveUpTime)
                context.Transition(context.idlePatrolState);
        }
    }
}
