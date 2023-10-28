using UnityEngine;

class Grounded : State {
    public Grounded(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Grounded"; } }

    public override void Start() {
        base.Start();
        EventManager.singleton.AddEvent(new ChangedGroundstatemsg(pm.gameObject, true));
        pm.numOfWallJumps = 0;
        pm.dashesRemaining = pm.dashes;
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) {
        base.Update(frim);

        //state change
        if (!pm.isGrounded)
        {
            SetState("Airborne");
            return;
        }

        //shooting
        if (frim.ShootButton)
        {
            EventManager.singleton.AddEvent(new playerShootGunmsg(0));
        }

        //movement handling
        float horizontal = 0;
        if (frim.RightButton && frim.LeftButton)
        {
            horizontal = 0;
        }
        else if (frim.RightButton)
        {
            horizontal = 1;
            pm.model.localScale = new Vector3(1, 1, 1); //right
        }
        else if (frim.LeftButton)
        {
            horizontal = -1;
            pm.model.localScale = new Vector3(-1f, 1, 1); //left
        }

        if (frim.UpButton)
        {
            pm.rb.velocity = new Vector2(horizontal * pm.speed, pm.jumpPower);
            EventManager.singleton.AddEvent(new Jumpmsg(pm.gameObject));
            SetState("Airborne");
            return;
        }

        if (pm.rb.velocity.x != 0 && horizontal == 0) //if it was moving previous frame and stopped in this
        {
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, false));
        }
        else if (pm.rb.velocity.x == 0 && horizontal != 0) //if it was stopped previous frame and moving in this
        {
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, true));
        }

        pm.rb.velocity = new Vector2(horizontal * pm.speed, 0); //sets jump
    }

    public override void Exit() {
        EventManager.singleton.AddEvent(new ChangedGroundstatemsg(pm.gameObject, false));
    }
}