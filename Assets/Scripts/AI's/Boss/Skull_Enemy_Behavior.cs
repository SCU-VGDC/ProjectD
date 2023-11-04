using UnityEngine;
using System.Collections;

//TODO: Doesn't need seeker or mover, probably change how the inheritence works.
public class Skull_Enemy_Behavior : Base_Enemy
{
    [Header("Properties")]
    public LayerMask hittableLayers;
    public bool completed_path;
    public Vector3 nextPos;

    [Header("States")]
    public SimpleChaseState pursue_state;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();

        pursue_state = new SimpleChaseState(GameManager.inst.player.transform, speed);

        // Flyswarm only has pursue state, so it will never change from this
        current_state = pursue_state;
        current_state.Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        completed_path = seeker.GetCurrentPath() == null || mover.reachedEndOfPath;

        current_state.Action(this);
    }
}