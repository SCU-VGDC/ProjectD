using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Behavior : Base_Enemy
{
    public Rigidbody2D rb;
    public Move_Forward_State move_state;
    public bool ricochet = false;
    [SerializeField]
    int maxRicochetCount = 3;
    [SerializeField]
    int currentRicochetCount = 0;
    [SerializeField]
    public LayerMask ricochetLayers;
    [HideInInspector]
    public RaycastHit2D[] ricochetHits = new RaycastHit2D[1];
    bool ricocheted = false;

    private void Start()
    {
        base.Init();

        //Ricochet needs to make sure not to be destroyed on contact!
        /*if (ricochet)
            destroyOnContact = false;*/
        move_state = new Move_Forward_State(transform.rotation);
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

    public void OnTriggerEnter2D(Collider2D collider)
    {
        //Ricochet!
        Debug.Log("it enters");
        if (ricochet && !ricocheted && Helpers.MatchesLayerMask(collider.gameObject, ricochetLayers) && currentRicochetCount < maxRicochetCount)
        {
            Debug.Log("it goes into condition");
            destroyOnContact=false;
            ricocheted = true;
            Quaternion reflectedRot = Quaternion.FromToRotation(transform.right, Vector2.Reflect(transform.right, -ricochetHits[0].normal)) * transform.rotation;
            transform.rotation = reflectedRot;
            move_state = new Move_Forward_State(reflectedRot);
            current_state = move_state;
            currentRicochetCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ricocheted = false;
        destroyOnContact=true;
    }
}

[System.Serializable]
public class Move_Forward_State : AI_State
{
    Quaternion startRotation;
    ContactFilter2D ricochetFilter;

    public Move_Forward_State(Quaternion inputRotation)
    {
        startRotation = inputRotation;
    }

    public override void Init(Base_Enemy context)
    {
        ricochetFilter = new ContactFilter2D();
        ricochetFilter.SetLayerMask(((Bullet_Behavior)context).ricochetLayers);
    }

    public override void Action(Base_Enemy context)
    {
        Vector3 dir = context.transform.right;

        //Get ricochet direction BEFORE you potentially move into the collider.
        Physics2D.Raycast(context.transform.position, dir, ricochetFilter, ((Bullet_Behavior)context).ricochetHits, 1f);

        context.mover.FinalizeMovement(context.transform.position + (dir * context.speed * Time.deltaTime), startRotation);
    }

    public override void OnDrawGizmos(Base_Enemy context)
    {
        Gizmos.DrawRay(new Ray(context.transform.position, context.transform.right));
    }
}

