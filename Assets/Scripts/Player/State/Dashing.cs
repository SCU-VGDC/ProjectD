using UnityEngine;

class Dashing : PlayerState
{
    private bool dashButtonReleased = true;
    private Vector3 dashDirection;
    private Vector3 startVelocity;
    private Vector3 targetVelocity;

    public Dashing(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Dashing"; } }

    public override bool CanStart(PlayerMov_FSM.FrameInput frim) 
    {
        if (pm.currentState == this)
            return true;

        if (!frim.DashButton) 
        {
            dashButtonReleased = true;
        }

        if (frim.DashButton && dashButtonReleased && pm.dashesRemaining > 0) 
        {
            // Save the dash direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            dashDirection = mousePos - pm.transform.position;
            dashDirection.Normalize();

            dashButtonReleased = false;
            pm.dashesRemaining--;

            return true;
        }

        return false;
    }

    public override void Start() 
    {
        base.Start();

        // Set initial and target velocities
        startVelocity = pm.rb.velocity;
        targetVelocity = dashDirection * pm.dashSpeed;

        EventManager.singleton.AddEvent(new Dashmsg(pm.gameObject));
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) 
    {
        base.Update(frim);

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
            pm.rb.velocity = Vector3.Lerp(targetVelocity, dashDirection * pm.dashSpeed * 0.2f, decelerationProgress);
        }

        if (StateTimeExceeds(pm.dashTime)) 
        {
            SetState("Airborne");
        }
    }
}
