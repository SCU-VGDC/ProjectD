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
	public float speed = 3;

    public AI_State current_state;

	[HideInInspector]
	public Seeker seeker;
	[HideInInspector]
	public AILerp mover;

	public bool dealsContactDamage;
	public int contactDamageAmount;

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

	public void OnTriggerEnter2D(Collider2D collider2D)
    {
		if (dealsContactDamage && collider2D.tag == "Player")
        {
			GameManager.inst.playerHealth.ApplyDamage(contactDamageAmount);
        }
    }
}
