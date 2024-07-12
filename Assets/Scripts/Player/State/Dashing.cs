using UnityEngine;

class Dashing : PlayerState
{
    private Vector3 dashDirection;
    private Vector3 startVelocity;
    private Vector3 targetVelocity;

    public Dashing(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Dashing"; } }

    public override bool CanStart(PlayerMov_FSM.FrameInput frim) 
    {
        if (pm.currentState == this)
            return !StateTimeExceeds(pm.dashTime);

        return false;
    }

    public override void Start() 
    {
        base.Start();

        GameManager.inst.player.GetComponent<ActorHealth>().isInvincible = true;

        // Save the dash direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        dashDirection = mousePos - pm.transform.position;
        dashDirection.Normalize();
        pm.dashesRemaining--;
        EventManager.singleton.AddEvent(new Dashmsg(pm.gameObject));

        // Set initial and target velocities
        startVelocity = pm.rb.velocity;
        targetVelocity = dashDirection * pm.dashSpeed;
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) 
    {
        base.Update(frim);

        // Prevent pm from recovering dashes during Dashing state
        pm.lastDashUpdate = Time.time;

        float progress = timeInState / pm.dashTime;
        
        if (progress <= 0.8f) 
        {
            // Lerp from start velocity to target dash velocity based on dash progress
            pm.rb.velocity = Vector3.Lerp(startVelocity, targetVelocity, progress / 0.8f);
        } 
        else 
        {
            // Smoothly decelerate using a curve
            float decelerationProgress = (progress - 0.8f) / 0.2f;
            pm.rb.velocity = Vector3.Lerp(targetVelocity, 0.2f * pm.dashSpeed * dashDirection, decelerationProgress);
        }
    }

    public override void Exit()
    {
        base.Exit();

        GameManager.inst.player.GetComponent<ActorHealth>().isInvincible = false;
    }
}
