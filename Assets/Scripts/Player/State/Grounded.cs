using UnityEngine;
using Cinemachine;
using System.Linq;

class Grounded : PlayerState
{
    public Grounded(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Grounded"; } }

    //Virtual Camera tracked object offset x and y value
    public float vc_tracked_x;
    public float vc_tracked_y;
    public override bool CanStart(PlayerMov_FSM.FrameInput frim)
    {
        return pm.isGrounded;
    }

    public override void Start()
    {
        base.Start();
        EventManager.singleton.AddEvent(new ChangedGroundstatemsg(pm.gameObject, true));
        EventManager.singleton.AddEvent(new Jumpmsg(pm.gameObject, false));//this code is to make sure it stops the jump animation trigger.
        pm.numOfWallJumps = 0;

        //get and store Virtual Camera tracked object offset x value
        vc_tracked_x = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset.x;

        //get and store Virtual Camera tracked object offset y value
        vc_tracked_y = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset.y;
        pm.GetState<OnWall>().lastWallNormal = Vector2.zero;
        pm.GetState<OnWall>().currentWallNormal = Vector2.zero;
    }

    public override void Update(PlayerMov_FSM.FrameInput frim)
    {
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

        //movement handling

        if (frim.DownButton) //for semisolids
        {
            EventManager.singleton.AddEvent(new playerPressedCrouch(pm.gameObject));
        }

        float horizontal = 0;
        if (frim.RightButton && frim.LeftButton)
        {
            horizontal = 0;
        }
        else if (frim.RightButton)
        {
            horizontal = 1;

            //change lookahead to face right
            vc_tracked_x = 2;
        }
        else if (frim.LeftButton)
        {
            horizontal = -1;

            //chnage lookahead to face left
            vc_tracked_x = -2;
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
        else if(pm.rb.velocity.x >0 && horizontal>0)//when you go to the air then lands and then your press the left button this allows it to keep the animation instead of gliding.
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
        else if(pm.rb.velocity.x <0 && horizontal<0)//when you go to the air then lands and then lands and then your press the right button this allows it to keep the animation instead of glidin
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
       
        else if(pm.rb.velocity.x==0) // this is to check whether or not it hit a wall. And then if you stop it'll end the animation.
        {
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(pm.gameObject, false));
            EventManager.singleton.AddEvent(new ChangedMOVBackstatemsg(pm.gameObject, false));
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