using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

public class SimpleChaseState : AI_State
{
    private Transform target;
    float speed = 15f;
    SpriteRenderer renderer;

    public SimpleChaseState(Transform _target, float _speed)
    {
        target = _target;
        speed = _speed;
    }

    public override void Init(Base_Enemy context)
    {
        renderer = context.GetComponent<SpriteRenderer>();
    }

    public override void Action(Base_Enemy context)
    {
        bool flip = (target.position.x - context.transform.position.x) < 0;

        //When attacking, face direction the player
        //context.transform.localScale = (target.position.x - context.transform.position.x > 0) ? flippedScale : initialScale;

        context.transform.position = Vector3.MoveTowards(context.transform.position, target.position, Time.deltaTime * speed);

        renderer.flipX = flip;

        Vector3 offset = target.position - context.transform.position;
        // Construct a rotation as in the y+ case.
        context.transform.rotation = Quaternion.LookRotation(Vector3.forward, offset) * Quaternion.Euler(0, 0, 90);
        if (flip) context.transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
    }
}
