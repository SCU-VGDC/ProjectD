using UnityEngine;

class Airborne : State
{
    public Airborne(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Airborne"; } }

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

        if (!pm.isGrounded && (pm.isWallLeft || pm.isWallRight))
        {
            ChangeState("OnWall");
            return;
        }
        else if (pm.isGrounded)
        {
            ChangeState("Grounded");
            return;
        }

        float horizontal = 0;
        if (frim.RightButton == frim.LeftButton)
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

        float movement = horizontal * pm.gladingSpeed;

        pm.rb.velocity = pm.ApplyGravity(frim.UpButton, new Vector2(movement, pm.rb.velocity.y));
    }
}