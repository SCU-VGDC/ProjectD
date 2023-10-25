using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Util;

[RequireComponent(typeof(AILerp))]
[RequireComponent(typeof(Seeker))]
/**
 * Mandates all components that all enemies will need (Pathfinder and basic locomotion).
 * Also contains basic information such as speed and the current state that the AI is in.
 **/
public abstract class Base_Enemy : MonoBehaviour
{
	public string PrefabName;

	public float speed = 3;

    public AI_State current_state;

	[HideInInspector]
	public Seeker seeker;
	[HideInInspector]
	public AILerp mover;

	[Header("Contact Damage")]
	public bool dealsContactDamage = false;
	public int contactDamageAmount;
	public LayerMask contactLayers;
	public bool destroyOnContact = false;

	public void Init()
    {
		seeker = GetComponent<Seeker>();
		mover = GetComponent<AILerp>();
	}

	/**
	 * 
	 **/
	public void Transition(AI_State target)
    {
        current_state = target;
        current_state.Init(this);
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!dealsContactDamage) return;

        //This looks complicated, but just checks if the collided object is within our hittable layers.
        if ((contactLayers & (1 << collision.gameObject.layer)) != 0)
		{
			if (collision.gameObject.tag == "Player")
			{
                EventManager.singleton.AddEvent(new meleeDamagemsg(gameObject, GameManager.inst.player.transform, contactDamageAmount));
			}

			if (destroyOnContact)
            {
				Destroy(gameObject);
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collider2D)
    {
		if (collider2D.tag == "Player")
        {
			if (GameManager.inst.playerMovement.currentState == "Dash") //player is dashing thus provoking damage
			{
				//DASH DAMAGE SET HERE
				EventManager.singleton.AddEvent(new applyDamagemsg(collider2D.gameObject, GetComponent<ActorHealth>(), 1));
				return;
			}
        }
    }
}
