class WallJumping : PlayerState {
    public WallJumping(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "WallJumping"; } }

    public override void Start() {
        base.Start();
        EventManager.singleton.AddEvent(new ChangedGroundstatemsg(pm.gameObject, false));
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) {
        base.Update(frim);

        pm.rb.velocity = pm.ApplyGravity(frim.UpButton, pm.rb.velocity);

        if (StateTimeExceeds(pm.wallJumpTime)) {
            SetState("Airborne");
        }
    }
}