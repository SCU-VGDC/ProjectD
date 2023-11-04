using UnityEngine;

class OnWall : PlayerState
{
    public OnWall(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "OnWall"; } }

    public override void Start() {
        base.Start();
        pm.numOfWallJumps++;
        EventManager.singleton.AddEvent(new ChangedWallstatemsg(pm.gameObject, true));
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) {
        base.Update(frim);

        //shooting
        bool isFacingRight = (frim.armRotation < 90 && frim.armRotation > -90);

        if (!pm.isGrounded && !pm.isWallLeft && !pm.isWallRight) //is not touching anything
        {
            SetState("Airborne");
        }
        else if (pm.isGrounded)
        {
            SetState("Grounded");
            return;
        }

        if (pm.isWallLeft)
        {
            pm.model.localScale = new Vector3(1, 1, 1); //right
            if (frim.ShootButton && isFacingRight)
            {
                EventManager.singleton.AddEvent(new playerShootGunmsg(0));
            }
        }

        if (pm.isWallRight)
        {
            pm.model.localScale = new Vector3(-1f, 1, 1); //left
            if (frim.ShootButton && !isFacingRight)
            {
                EventManager.singleton.AddEvent(new playerShootGunmsg(0));
            }
        }

        if (frim.RightButton && pm.isWallLeft)
        {
            pm.rb.velocity = new Vector2(pm.speed, 0);
            SetState("Airborne");
            return;
        }

        if (frim.LeftButton && pm.isWallRight)
        {
            pm.rb.velocity = new Vector2(-pm.speed, 0);
            SetState("Airborne");
            return;
        }

        if (pm.mJump != frim.UpButton) //checks that jump button was released before
        {
            if (frim.UpButton && pm.numOfWallJumps <= pm.maxWallJumps)
            {
                if (pm.isWallLeft)
                {
                    pm.rb.velocity = new Vector2(pm.wallSideJumpX, pm.wallSideJumpY * 2); //right
                    EventManager.singleton.AddEvent(new Jumpmsg(pm.gameObject));
                    SetState("WallJumping");
                    return;
                }

                else if (pm.isWallRight)
                {
                    pm.rb.velocity = new Vector2(-pm.wallSideJumpX, pm.wallSideJumpY * 2); //left
                    EventManager.singleton.AddEvent(new Jumpmsg(pm.gameObject));
                    SetState("WallJumping");
                    return;
                }

            }
        }

        //one wall contact see
        if (pm.rb.velocity.y < 0)
        {
            pm.rb.velocity = new Vector2(0, -pm.wallSlidingSpeed);

        }
        else
        {
            pm.rb.velocity = pm.ApplyGravity(frim.UpButton);
        }
    }

    public override void Exit() {
        base.Exit();
        EventManager.singleton.AddEvent(new ChangedWallstatemsg(pm.gameObject, false));
    }
}