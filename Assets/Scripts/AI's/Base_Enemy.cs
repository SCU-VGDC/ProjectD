using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Util;

[RequireComponent(typeof(AILerp))]
[RequireComponent(typeof(Seeker))]
public abstract class Base_Enemy : MonoBehaviour
{
	public float speed = 3;

    public AI_State current_state;

	[HideInInspector]
	public Seeker seeker;
	[HideInInspector]
	public AILerp mover;

	public void Init()
    {
		seeker = GetComponent<Seeker>();
		mover = GetComponent<AILerp>();
	}

	public void Transition(AI_State target)
    {
        current_state = target;
        current_state.Init(this);
    }
}
