using UnityEngine;

public class DashCharging : PlayerState
{
    public DashCharging(PlayerMov_FSM playerMov) : base(playerMov) { }

    private bool inputAllowed = true;
    private Vector2 lastVel;
    private float startGrav;
    private ParticleSystem dashChargeParticle;
    public override string name { get { return "Dashing"; } }

    public override bool CanStart(PlayerMov_FSM.FrameInput frim)
    {
        if(!frim.DashButton)
        {
            inputAllowed = pm.dashesRemaining > 0;
            return false;
        }

        return inputAllowed || pm.currentState == this;
    }

    public override void Start()
    {
        base.Start();
        inputAllowed = false;
        lastVel = pm.rb.velocity;
        startGrav = pm.gravity;
        pm.gravity = pm.chargeGravity;

        //find particle system
        dashChargeParticle = GameObject.Find("Dash Charge Particle System").GetComponent<ParticleSystem>();

        //stop particle system
        dashChargeParticle.Stop();

        //play particle system
        dashChargeParticle.Play();
    }

    public override void Exit()
    {
        base.Exit();
        pm.gravity = startGrav;

        //stop particle system
        dashChargeParticle.Stop();
    }

    // Update is called once per frame
    public override void Update(PlayerMov_FSM.FrameInput frim)
    {
        base.Update(frim);
        lastVel = pm.ApplyGravity(false, Vector2.Lerp(lastVel, lastVel * (1-pm.chargeVelDecay), timeInState / pm.chargeTime));
        pm.rb.velocity = lastVel;

        // Prevent pm from recovering dashes during Dashing state
        pm.lastDashUpdate = Time.time;

        if (StateTimeExceeds(pm.chargeTime))
            pm.SetState<Dashing>();
    }
}
