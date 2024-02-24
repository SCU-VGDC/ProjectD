using UnityEngine;

public class DashCharging : PlayerState
{
    public DashCharging(PlayerMov_FSM playerMov) : base(playerMov) { }

    private bool dashReady = true;
    private Vector2 lastVel;
    private float startGrav;
    public override string name { get { return "Dashing"; } }

    public override bool CanStart(PlayerMov_FSM.FrameInput frim)
    {
        if (frim.DashButton)
            return dashReady || pm.currentState == this;
        else
        {
            dashReady = true;
            return false;
        }
    }

    public override void Start()
    {
        base.Start();
        dashReady = false;
        lastVel = pm.rb.velocity;
        startGrav = pm.gravity;
        pm.gravity = pm.chargeGravity;
    }

    public override void Exit()
    {
        base.Exit();
        pm.gravity = startGrav;
    }

    // Update is called once per frame
    public override void Update(PlayerMov_FSM.FrameInput frim)
    {
        base.Update(frim);
        lastVel = pm.ApplyGravity(false, Vector2.Lerp(lastVel, lastVel * (1-pm.chargeVelDecay), timeInState / pm.chargeTime));
        pm.rb.velocity = lastVel;

        if (StateTimeExceeds(pm.chargeTime))
            pm.SetState<Dashing>();
    }
}
