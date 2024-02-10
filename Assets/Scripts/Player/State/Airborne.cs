using UnityEngine;

class Airborne : PlayerState
{
    public Airborne(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Airborne"; } }

    public override bool CanStart(PlayerMov_FSM.FrameInput _)
    {
        return !pm.isGrounded;
    }

    public override void Start() {
        base.Start();
        EventManager.singleton.AddEvent(new ChangedGroundstatemsg(pm.gameObject, false));
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) {
        base.Update(frim);

        //shooting
        if (frim.ShootButton)
        {
            EventManager.singleton.AddEvent(new playerShootGunmsg(0));
        }
        if (frim.ShootAltButton)
        {
            EventManager.singleton.AddEvent(new playerShootGunmsg(1));
        }

        bool isFacingRight = (frim.armRotation < 90 && frim.armRotation > -90);
        if (isFacingRight)
        {
            pm.model.localScale = new Vector3(1f, 1f, 1f); //right
        }
        else
        {
            pm.model.localScale = new Vector3(-1f, 1f, 1f); ; //left
        }

        float horizontal = 0;
        if (frim.RightButton == frim.LeftButton)
        {
            horizontal = 0;
        }
        else if (frim.RightButton)
        {
            horizontal = 1;
        }
        else if (frim.LeftButton)
        {
            horizontal = -1;
        }

        float movement = horizontal * pm.gladingSpeed;

        pm.rb.velocity = pm.ApplyGravity(frim.UpButton, new Vector2(movement, pm.rb.velocity.y));
    }
}