using System;
using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        Init();

        // Hide the endpoints in-game
        foreach (Renderer renderer in point1.GetComponents<Renderer>()) Destroy(renderer);
        foreach (Renderer renderer in point2.GetComponents<Renderer>()) Destroy(renderer);

        leftX = MathF.Min(point1.position.x, point2.position.x);
        rightX = MathF.Max(point1.position.x, point2.position.x);
        idlePatrolState.chase = wasteChaseState;
        idlePatrolState.leftX = leftX;
        idlePatrolState.rightX = rightX;

        wasteChaseState.idle = idlePatrolState;
        wasteChaseState.leftX = leftX;
        wasteChaseState.rightX = rightX;

        current_state = idlePatrolState;
        current_state.Init(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        current_state.Action(this);
    }

    [Serializable]
    public class Waste_Idle_Patrol : AI_State
    {
        private float walkTime = 0;
        private float walkTimeGoal = 0;
        private int direction = -1;
        private int dDelta = 1;
        [NonSerialized] public Waste_Chase chase;
        [NonSerialized] public float leftX;
        [NonSerialized] public float rightX;
        public float minIdleWalkTime = 0.5f;
        public float maxIdleWalkTime = 4f;
        public float aggroRange = 5;
        public bool aggroOnlyInFront = false;

        public override void Init(Base_Enemy context)
        {
            walkTime = 0;
        }
        public override void Action(Base_Enemy context)
        {
            bool oobLeft = context.transform.position.x < leftX;
            bool oobRight = context.transform.position.x > rightX;
            walkTime += Time.deltaTime;
            if (walkTime >= walkTimeGoal || oobLeft || oobRight)
            {
                direction = UnityEngine.Random.Range(oobLeft ? 1 : -1, oobRight ? 0 : 2);
                walkTime = 0;
                walkTimeGoal = UnityEngine.Random.Range(minIdleWalkTime, maxIdleWalkTime);
            }
            context.transform.position += new Vector3(context.speed * direction * Time.deltaTime, 0);

            Vector2 playerPos = GameManager.inst.player.transform.position;
            int playerDir = Math.Sign(playerPos.x - context.transform.position.x);
            bool inRange = Vector2.Distance(playerPos, context.transform.position) <= aggroRange;
            bool inFront = playerDir == direction;
            if (inRange && (!aggroOnlyInFront || inFront))
                context.Transition(chase);
        }
    }

    [Serializable]
    public class Waste_Chase : AI_State
    {
        private float giveUpTimeCounter = 0;
        [NonSerialized] public Waste_Idle_Patrol idle;
        [NonSerialized] public float leftX;
        [NonSerialized] public float rightX;
        public float giveUpRange = 10;
        public float giveUpTime = 2;
        public float aggroSpeedMult = 1.4f;

        public override void Init(Base_Enemy context)
        {
            giveUpTimeCounter = 0;
        }
        public override void Action(Base_Enemy context)
        {
            Transform playerTrans = GameManager.inst.player.transform;
            float xdiff = playerTrans.position.x - context.transform.position.x;
            // Stop waste from having a seizure when under/near the player
            if (MathF.Abs(xdiff) > 0.8f)
            {
                int direction = Math.Sign(xdiff);
                float rawX = context.transform.position.x + (context.speed * aggroSpeedMult * direction * Time.deltaTime);
                context.transform.position = new Vector2(Mathf.Clamp(rawX, leftX, rightX), context.transform.position.y);
            }

            bool plrOutOfRange = Vector2.Distance(playerTrans.position, context.transform.position) > giveUpRange;
            giveUpTimeCounter = plrOutOfRange ? giveUpTimeCounter + Time.deltaTime : 0;
            if (giveUpTimeCounter > giveUpTime)
                context.Transition(idle);
        }
    }
}
