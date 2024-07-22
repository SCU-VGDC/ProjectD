using UnityEngine;

class Knockback : PlayerState
{
    private Vector3 dashDirection;
    private Vector3 startVelocity;
    private Vector3 targetVelocity;

    public Knockback(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Knockback"; } }

    public override bool CanStart(PlayerMov_FSM.FrameInput frim) 
    {
        if (pm.currentState == this)
            return !StateTimeExceeds(pm.knockbackTime);

        return false;
    }

    public override void Start() 
    {
        base.Start();

        // Save the dash direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        dashDirection = mousePos - pm.transform.position;
        dashDirection = dashDirection * -1f;
        dashDirection.Normalize();
        pm.knockbacksRemaining--;
        //EventManager.singleton.AddEvent(new Dashmsg(pm.gameObject));

        // Set initial and target velocities
        startVelocity = pm.rb.velocity;
        targetVelocity = dashDirection * pm.knockbackSpeed;
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) 
    {
        base.Update(frim);
        Debug.Log("Here");

        float progress = timeInState / pm.knockbackTime;

        if (progress <= 0.8f) 
        {
            // Lerp from start velocity to target dash velocity based on dash progress
            pm.rb.velocity = Vector3.Lerp(startVelocity, targetVelocity, progress / 0.8f);
        } 
        else 
        {
            // Smoothly decelerate using a curve
            float decelerationProgress = (progress - 0.8f) / 0.2f;
            pm.rb.velocity = Vector3.Lerp(targetVelocity, 0.2f * pm.knockbackSpeed * dashDirection, decelerationProgress);
        }
    }
}