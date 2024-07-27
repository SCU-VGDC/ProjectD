using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SniperBullet_Beh : Base_Enemy
{
    //TODO GABE do you like it?

    public Rigidbody2D rb;
    public Move_Forward_Statesniper move_state;

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
    
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.LogError(collision.gameObject);
        if(collision.gameObject.layer == 6) //this checks if it hits a platform.
            Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
            Destroy(gameObject);
    }
   
}

[System.Serializable]
public class Move_Forward_Statesniper : AI_State
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