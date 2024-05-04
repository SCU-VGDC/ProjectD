using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;

[RequireComponent(typeof(Collider2D))]
public class Mobster_Enemy_Behavior : Base_Enemy
{
    //public float aggroRange = 40f;
    //public float attackRangeMin = 10f;
    //public float attackRangeMax = 25f;
    [NonSerialized] public float currentSpeedMult = 1f;
    public float projectileSpeed = 110f;
    private float leftXLimit;
    private float rightXLimit;
    public Transform limitPoint1;
    public Transform limitPoint2;
    public Mobster_Idle_Wander idleWanderState;
    public Mobster_Chase chaseState;
    public Mobster_Reposition repositionState;
    public int WalkDirection { get; private set; } = 0;
    
    // Start is called before the first frame update
    private void Start()
    {
        Init();
        current_state = idleWanderState;
        current_state.Init(this);

        // Hide the endpoints in-game
        foreach (Renderer renderer in limitPoint1.GetComponents<Renderer>()) Destroy(renderer);
        foreach (Renderer renderer in limitPoint2.GetComponents<Renderer>()) Destroy(renderer);

        leftXLimit = MathF.Min(limitPoint1.position.x, limitPoint2.position.x);
        rightXLimit = MathF.Max(limitPoint1.position.x, limitPoint2.position.x);
    }

    private void FixedUpdate()
    {
        current_state.Action(this);
        float rawX = transform.position.x + (speed * currentSpeedMult * WalkDirection * Time.deltaTime);
        transform.position = new Vector2(Mathf.Clamp(rawX, leftXLimit, rightXLimit), transform.position.y);
    }

    private void OnDrawGizmos()
    {
        if (current_state == chaseState && chaseState.gizmoShootVector != Vector2.zero)
            Gizmos.DrawRay(transform.position, chaseState.gizmoShootVector);
        if (current_state == chaseState)
        {
            Gizmos.DrawSphere(new Vector3(chaseState.gizmoGoalX, transform.position.y, 0), 0.5f);
        }
    }

    public void SetDirection(int nextDir)
    {
        if (nextDir == WalkDirection) return;

        if (nextDir == 0)
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(gameObject, false));
        else if (WalkDirection == 0)
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(gameObject, true));

        SpriteRenderer spriteR = GetComponent<SpriteRenderer>();
        spriteR.flipX = nextDir != 0 ? nextDir == -1 : spriteR.flipX;
        WalkDirection = nextDir;
    }
    
    [Serializable]
    public class Mobster_Idle_Wander : AI_State
    {
        public float aggroRange = 40f;
        public float minWalkTime = 1f;
        public float maxWalkTime = 6f;
        public float speedMult = 0.8f;
        private float walkTimeRemaining;

        public override void Init(Base_Enemy context)
        {
            // GARY THERE'S A BOMB STRAPPED TO MY CHEST AND IT'LL EXPLODE IN 5 SECONDS
            ((Mobster_Enemy_Behavior)context).currentSpeedMult = speedMult;
        }

        public override void Action(Base_Enemy baseContext)
        {
            Mobster_Enemy_Behavior context = (Mobster_Enemy_Behavior)baseContext;
            Transform plr = GameManager.inst.player.transform;
            if (Vector2.Distance(plr.position, context.transform.position) <= aggroRange)
                context.Transition(context.chaseState);
            else
            {
                bool oobLeft = context.transform.position.x <= context.leftXLimit;
                bool oobRight = context.transform.position.x >= context.rightXLimit;
                if (!oobLeft && !oobRight)
                    if (walkTimeRemaining <= 0)
                    {
                        context.SetDirection(UnityEngine.Random.Range(-1, 2));
                        walkTimeRemaining = UnityEngine.Random.Range(minWalkTime, maxWalkTime);
                    }
                else
                {
                    context.SetDirection(oobLeft ? 1 : -1);
                    walkTimeRemaining = UnityEngine.Random.Range(minWalkTime, maxWalkTime);
                }
                walkTimeRemaining -= Time.deltaTime;
            }
        }
    }

    [Serializable]
    public class Mobster_Chase : AI_State
    {
        public float minAttackRange = 10f;
        public float maxAttackRange = 25f;
        public float waitAfterShoot = 0.5f;
        public float speedMult = 1.8f;
        private bool readyToRepos = false;
        private float goalDist;
        private float lastShotTime;
        public Vector2 gizmoShootVector = Vector2.zero;
        public float gizmoGoalX;

        public override void Init(Base_Enemy context)
        {
            readyToRepos = false;
            lastShotTime = 0;
            ((Mobster_Enemy_Behavior)context).currentSpeedMult = speedMult;
            gizmoShootVector = Vector2.zero;
            goalDist = UnityEngine.Random.Range(minAttackRange, maxAttackRange);
        }

        public override void Action(Base_Enemy baseContext)
        {
            Mobster_Enemy_Behavior context = (Mobster_Enemy_Behavior)baseContext;
            Transform plr = GameManager.inst.player.transform;

            if (readyToRepos)
            {
                if (Time.time - lastShotTime > waitAfterShoot)
                    context.Transition(context.repositionState);
                return;
            }

            Vector2 playerToMob = context.transform.position - plr.position;
            float goalX = plr.position.x + goalDist * MathF.Sign(playerToMob.x);
            float toGoalX = goalX - context.transform.position.x;
            bool oobLeft = context.transform.position.x <= context.leftXLimit && goalX <= context.leftXLimit;
            bool oobRight = context.transform.position.x >= context.rightXLimit && goalX >= context.rightXLimit;
            gizmoGoalX = goalX;

            if (Mathf.Abs(toGoalX) <= 0.5f || oobLeft || oobRight)
            {
                context.SetDirection(0);
                //EventManager.singleton.AddEvent(new shootmsg(context.gameObject, plr));
                gizmoShootVector = plr.position - context.transform.position;
                Debug.Log("mobster shoot");
                readyToRepos = true;
                lastShotTime = Time.time;
            }
            else
                context.SetDirection(MathF.Sign(toGoalX));
        }
    }

    [Serializable]
    public class Mobster_Reposition : AI_State
    {
        public float minRelocateDistance = 5f;
        public float maxRelocateDistance = 25f;
        public float speedMult = 2.2f;
        private float startX;
        private float goalDist;
        public override void Init(Base_Enemy baseContext)
        {
            Mobster_Enemy_Behavior context = (Mobster_Enemy_Behavior)baseContext;
            context.currentSpeedMult = speedMult;
            context.SetDirection(MathF.Sign((context.transform.position - GameManager.inst.player.transform.position).x));
            goalDist = UnityEngine.Random.Range(minRelocateDistance, maxRelocateDistance);
            startX = context.transform.position.x;
        }

        public override void Action(Base_Enemy baseContext)
        {
            Mobster_Enemy_Behavior context = (Mobster_Enemy_Behavior)baseContext;
            bool reachedGoal = MathF.Abs(context.transform.position.x - startX) >= goalDist;
            bool oobLeft = context.transform.position.x <= context.leftXLimit;
            bool oobRight = context.transform.position.x >= context.rightXLimit;
            if (reachedGoal || oobLeft || oobRight)
            {
                context.Transition(context.chaseState);
            }
        }
    }
}
