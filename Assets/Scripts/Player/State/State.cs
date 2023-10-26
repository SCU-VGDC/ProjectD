abstract class State
{
    protected PlayerMov_FSM playerMovement;

    public State(PlayerMov_FSM playerMovement)
    {
        this.playerMovement = playerMovement;
    }

    public abstract string stateName { get; }
    public abstract void Start();
    public abstract void Update(PlayerMov_FSM.FrameInput frim);
    public virtual void Exit() { }
}