using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Mobster_Enemy_Behavior : Base_Enemy
{
    public float aggroRange = 40f;
    public float attackRange = 25f;
    public float projectileSpeed = 110f;
    public int reposPointCount = 6;
    public float collisionAvoidAngle = 40f;
    [Range(10, 180)] public float reposSearchAngle = 160f;
    public Mobster_Idle_Wander idleWanderState;
    public Mobster_Chase chaseState;
    public Mobster_Reposition repositionState;
    
    // Start is called before the first frame update
    private void Start()
    {
        Init();
        current_state = idleWanderState;
        current_state.Init(this);
    }

    private void FixedUpdate()
    {
        current_state.Action(this);
    }

    private void OnDrawGizmos()
    {
        if (current_state == null) return;
        current_state.OnDrawGizmos(this);
    }

    [Serializable]
    public class Mobster_Idle_Wander : AI_State
    {
        public override void Init(Base_Enemy context)
        {
            // Squidward is all alone
        }

        public override void Action(Base_Enemy baseContext)
        {
            Mobster_Enemy_Behavior context = (Mobster_Enemy_Behavior)baseContext;
            Transform plr = GameManager.inst.player.transform;
            if (Vector2.Distance(plr.position, context.transform.position) <= context.aggroRange)
                context.Transition(context.chaseState);
        }
    }

    [Serializable]
    public class Mobster_Chase : AI_State
    {
        public float waitAfterShoot = 0.5f;
        private bool readyToRepos = false;
        private float lastShotTime;
        public override void Init(Base_Enemy context)
        {
            readyToRepos = false;
            lastShotTime = 0;
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

            if (Vector2.Distance(plr.position, context.transform.position) <= context.attackRange)
            {
                EventManager.singleton.AddEvent(new shootmsg(context.gameObject, plr));
                readyToRepos = true;
                lastShotTime = Time.time;
            }
            else
            {

            }
        }
    }

    [Serializable]
    public class Mobster_Reposition : AI_State
    {
        public float jumpRange = 10;
        public float jumpDuration = 0.4f;
        public float gravity = 9.8f;

        private readonly List<Vector2> potentials = new();
        private Vector2 targetPos;
        private float jumpT = -1;
        private Vector2 v0;
        private Vector2 p0;

        public override void Init(Base_Enemy baseContext)
        {
            Mobster_Enemy_Behavior context = (Mobster_Enemy_Behavior)baseContext;
            Vector2 contextPos = (Vector2)context.transform.position;

            float anglePerPoint = context.reposSearchAngle / context.reposPointCount;
            potentials.Clear();
            for (int i = 0; i < context.reposPointCount; i++)
            {
                float angle = (i * anglePerPoint) - context.reposSearchAngle / 2f;
                float len = UnityEngine.Random.Range(3f, 8f);
                Vector2 offset = new(MathF.Cos(angle), MathF.Sin(angle));
                Vector2 airPoint = contextPos + (offset * len);
                RaycastHit2D hit = Physics2D.Raycast(airPoint, Vector2.up, Camera.main.orthographicSize * 2, LayerMask.GetMask("Platforms"));
                if (hit.collider != null)
                    potentials.Add(hit.point);
            }

            targetPos = potentials.Count > 0 ? potentials[UnityEngine.Random.Range(0, potentials.Count)] : contextPos + Vector2.up*5;
            jumpT = -1;
        }

        public override void Action(Base_Enemy baseContext)
        {
            Mobster_Enemy_Behavior context = (Mobster_Enemy_Behavior)baseContext;
            Vector2 contextPos = context.transform.position;

            if (Vector2.Distance(context.transform.position, targetPos) < jumpRange)
            {
                if (jumpT == -1)
                {
                    float vx0 = (targetPos.x - contextPos.x) / jumpDuration;
                    float vy0 = (targetPos.y - contextPos.y - 0.5f * -MathF.Abs(gravity) * MathF.Pow(jumpDuration, 2)) / jumpDuration;
                    v0 = new Vector2(vx0, vy0);
                    p0 = contextPos;
                    jumpT = 0;
                }
                else if (jumpT >= jumpDuration)
                {
                    Debug.Log("Jump done");
                    context.Transition(context.chaseState);
                    return;
                }

                Vector2 a = new(0, -Math.Abs(gravity));
                context.transform.position = p0 + (v0 * jumpT) + (0.5f * MathF.Pow(jumpT, 2) * a);
                jumpT += Time.deltaTime;
            }
            else
            {
                int dir = Math.Sign(targetPos.x - contextPos.x);
                context.transform.position += new Vector3(context.speed * Time.deltaTime * dir, 0);
            }
        }

        public override void OnDrawGizmos(Base_Enemy context)
        {
            Gizmos.color = Color.white;
            foreach (Vector2 point in potentials)
                Gizmos.DrawSphere(point, 1);
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(targetPos, Vector3.one);
        }
    }
}
