using UnityEngine;

public abstract class PlayerState
{
    protected PlayerMov_FSM pm;
    public float timeInState = 0;

    public PlayerState(PlayerMov_FSM pm)
    {
        this.pm = pm;
    }

    public abstract string name { get; }

    public virtual void Start() {
        timeInState = 0;
    }

    public virtual bool CanStart(PlayerMov_FSM.FrameInput frim) {
        return false;
    }
    
    public virtual void Update(PlayerMov_FSM.FrameInput frim) {
        timeInState += Time.deltaTime;
    }

    public virtual void Exit() { }

    protected void SetState(string name) {
        pm.SetState(name);
    }

    // Used for states that have a time limit
    protected bool StateTimeExceeds(float time) {
        return timeInState >= time;
    }
}