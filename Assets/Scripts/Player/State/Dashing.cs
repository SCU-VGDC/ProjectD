using UnityEngine;

class Dashing : State
{
    private bool dashButtonReleased = true;
    private Vector3 dashDirection;

    public Dashing(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Dashing"; } }

    public override bool CanStart(PlayerMov_FSM.FrameInput frim) {
        if (!frim.DashButton) {
            dashButtonReleased = true;
        }

        if (frim.DashButton && dashButtonReleased && pm.dashesRemaining > 0) {
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

    public override void Start() {
        base.Start();
        EventManager.singleton.AddEvent(new Dashmsg(pm.gameObject));
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) {
        base.Update(frim);

        pm.rb.velocity = dashDirection * pm.dashSpeed;

        if (StateTimeExceeds(pm.dashTime)) {
            // Set velocity to almost zero
            pm.rb.velocity = dashDirection * pm.dashSpeed * 0.2f;
            SetState("Airborne");
        }
    }
}