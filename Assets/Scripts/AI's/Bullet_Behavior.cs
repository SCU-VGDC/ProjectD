using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Behavior : Base_Enemy
{
    public Rigidbody2D rb;
    public Move_Forward_State move_state;

    private void Start()
    {
        base.Init();

        current_state = move_state;
        current_state.Init(this);
    }

    private void Update()
    {
        current_state.Action(this);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        current_state.OnDrawGizmos(this);
    }
}

[System.Serializable]
public class Move_Forward_State : AI_State
{
    Quaternion startRotation;

    public override void Init(Base_Enemy context)
    {
        startRotation = context.transform.rotation;
    }

    public override void Action(Base_Enemy context)
    {
        Vector3 dir = context.transform.right;
        context.mover.FinalizeMovement(context.transform.position + (dir * context.speed * Time.deltaTime), startRotation);
    }

    public override void OnDrawGizmos(Base_Enemy context)
    {
        Gizmos.DrawRay(new Ray(context.transform.position, context.transform.right));
    }
}

