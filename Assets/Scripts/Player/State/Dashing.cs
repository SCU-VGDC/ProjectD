using UnityEngine;

class Dashing : State
{
    public Dashing(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Dashing"; } }

    public override void Start() {
        base.Start();
        EventManager.singleton.AddEvent(new Dashmsg(pm.gameObject));
    }

    public override bool CanStart(PlayerMov_FSM.FrameInput frim) {
        if (!frim.DashButton) {
            pm.dashButtonReleased = true;
        }

        Debug.Log(pm.isDashing);

        if (frim.DashButton && pm.dashButtonReleased && !pm.isDashing && pm.dashesRemaining > 0) {
            // Save the dash direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            pm.dashDirection = mousePos - pm.transform.position;
            pm.dashDirection.Normalize();

            pm.isDashing = true;
            pm.dashButtonReleased = false;
            pm.dashesRemaining--;

            return true;
        }

        return false;
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) {
        base.Update(frim);

        pm.rb.velocity = pm.dashDirection * pm.dashSpeed;

        if (StateTimeExceeds(pm.dashTime)) {
            // Set velocity to almost zero
            pm.rb.velocity = pm.dashDirection * pm.dashSpeed * 0.2f;
            pm.isDashing = false;
            StateChange("Airborne");
        }
    }
}