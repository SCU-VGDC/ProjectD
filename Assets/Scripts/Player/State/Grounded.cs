using UnityEngine;

class Grounded : PlayerState
{
    public Grounded(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Grounded"; } }

    public override void Start()
    {
        base.Start();
        EventManager.singleton.AddEvent(new ChangedGroundstatemsg(pm.gameObject, true));
        pm.numOfWallJumps = 0;
        pm.dashesRemaining = pm.dashes;
    }

    public override void Update(PlayerMov_FSM.FrameInput frim)
    {
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

        }
        else if (frim.LeftButton)
        {
            horizontal = -1;

        }

        bool isFacingRight = (frim.armRotation < 90 && frim.armRotation > -90);
        bool wasFacingRight = (pm.model.localScale.x == 1f);


        if (pm.rb.velocity.x != 0 && horizontal == 0) //if it was moving previous frame and stopped in this
        {
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, false));
            EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, false));
        }
        else if (pm.rb.velocity.x <= 0 && horizontal > 0) //was stopped or moving left started moving right
        {
            if (isFacingRight) //turned right
            {
                EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, true));
                EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, false));
            }
            else //turned left
            {
                EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, true));
                EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, false));
            }
        }
        else if (pm.rb.velocity.x >= 0 && horizontal < 0) //was stopped or moving right started moving left
        {
            if (isFacingRight) //turned right
            {
                EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, true));
                EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, false));           
            }
            else //turned left
            {
                EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, true));
                EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, false));
            }
        }

        if (wasFacingRight != isFacingRight) //switched orientation
        {
            if (horizontal != 0) //and continue moving
            {
                if (isFacingRight) //turned right
                {
                    if (horizontal > 0) //moves right
                    {
                        EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, true));
                        EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, false));
                    }
                    if (horizontal < 0) //moves left
                    {
                        EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, true));
                        EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, false));                        
                    }
                }
                else //turned left
                {
                    if (horizontal > 0) //moves right
                    {
                        EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, true));
                        EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, false));                      
                    }
                    if (horizontal < 0) //moves left
                    {
                        EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, true));
                        EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, false));
                    }
                }
            }
        }



        if (isFacingRight)
        {
            pm.model.localScale = new Vector3(1f, 1f, 1f); //right
        }
        else
        {
            pm.model.localScale = new Vector3(-1f, 1f, 1f); ; //left
        }


        if (frim.UpButton)
        {
            pm.rb.velocity = new Vector2(horizontal * pm.speed, pm.jumpPower);
            EventManager.singleton.AddEvent(new Jumpmsg(pm.gameObject));
            SetState("Airborne");
            return;
        }

        pm.rb.velocity = new Vector2(horizontal * pm.speed, 0); //sets jump
    }

    public override void Exit()
    {
        EventManager.singleton.AddEvent(new ChangedGroundstatemsg(pm.gameObject, false));
        EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, false));
        EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, false));
    }
}