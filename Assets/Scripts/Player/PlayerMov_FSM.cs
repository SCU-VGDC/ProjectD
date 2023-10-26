using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class PlayerMov_FSM : MonoBehaviour
{
    public struct FrameInput
    {
        public bool RightButton;
        public bool LeftButton;
        public bool UpButton;
        public bool DownButton;

        public bool DashButton;
        public bool ShootButton;
        public bool ShootAltButton;

        public bool RespawnButton;
        public bool InteractButton;

        public float armRotation;
    }

    private bool StateMutex;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Transform model;

    private ContactFilter2D cfGround;
    private ContactFilter2D cfWallR;
    private ContactFilter2D cfWallL;

    // Make sure to update this array when adding new states
    private State[] states = new State[6];
    private string currentState;

    [HideInInspector]
    public bool isGrounded, isWallRight, isWallLeft;

    private bool mJump;
    private int numOfWallJumps;

    public float speed;
    public float jumpPower;
    public float gravity; //I am so sorry
    public float wallSlidingSpeed;
    public float tightJumpScale;
    public float wallSideJumpX;
    public float wallSideJumpY;
    public float wallJumpTime;
    public float gladingSpeed;
    public int maxWallJumps;
    public Transform arm;

    //those changes are retarded LEGACY???
    public int dashes;
    public float dashTime;
    public float dashSpeed;
    public int dashesRemaining;

    private bool isDashing = false;
    private bool dashButtonReleased = true;
    private Vector3 dashDirection;

    //public ContactFilter2D contactFilter;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        model = transform.Find("BodyModel");
        cfGround.SetNormalAngle(85, 95); //perpendicular to ground
        cfWallL.SetNormalAngle(0, 5); //perpendicular to left wall
        cfWallR.SetNormalAngle(175, 185); //perpendicual to right wall
    }

    private void FixedUpdate()
    {
        FrameInput thisFrame = InputHandler(); // this can be chnaged into AI
        //DebugPrintInpput(thisFrame);

        UpdateArmPos(thisFrame);
        //StateChange();
        StateHandling(thisFrame);

    }

    //--------------------------------Inputs--------------------------------------------\

    FrameInput InputHandler()
    {
        FrameInput thisFrame = new FrameInput { };

        thisFrame.RightButton = false;
        thisFrame.LeftButton = false;
        thisFrame.UpButton = false;
        thisFrame.DownButton = false;
        thisFrame.DashButton = false;
        thisFrame.ShootButton = false;

        thisFrame.armRotation = 0;

        thisFrame.UpButton = Input.GetButton("Up");
        thisFrame.DownButton = Input.GetButton("Crouch");
        thisFrame.RightButton = Input.GetButton("Right");
        thisFrame.LeftButton = Input.GetButton("Left");

        thisFrame.DashButton = Input.GetButton("Dash");
        thisFrame.ShootButton = Input.GetButton("Shoot");
        thisFrame.ShootAltButton = Input.GetButton("AltShoot");


        Vector3 Mouse_Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = Mouse_Pos - transform.position;
        float rotation_z = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        thisFrame.armRotation = rotation_z;

        return thisFrame;
    }

    void UpdateArmPos(FrameInput frim)
    {
        arm.localRotation = Quaternion.Euler(0, 0, frim.armRotation);
    }

    //--------------------------------States changes--------------------------------------------\

    void StateHandling(FrameInput frim)
    {
        AnyStateUpdate(frim); //return if any state have changed the state

        switch (currentState)
        {
            case "Dash":
                OnDashUpdate();
                break;
            case "OnGround":
                OnGroundUpdate(frim);
                break;
            case "OnWall":
                OnWallUpdate(frim);
                break;
            case "OnWallJumping":
                OnWallJumpingUpdate(frim);
                break;
            case "OnFly":
                OnFlyUpdate(frim);
                break;
            case "Death":
                OnDeathUpdate();
                break;
            default:
                // lol
                break;
        }

        if (frim.UpButton != mJump) //jump button state
        {
            mJump = frim.UpButton;
        }
    }

    public void StateChange(string nextState)
    {
        if (nextState == currentState) //beacuse same state
        {
            return;
        }

        if (StateMutex == true) //mutex
        {
            return;
        }

        //previous state hadling
        if (currentState == "OnGround")
        {
            EventManager.singleton.AddEvent(new ChangedGroundstatemsg(gameObject, false));
        }
        if (currentState == "OnWall")
        {
            EventManager.singleton.AddEvent(new ChangedWallstatemsg(gameObject, false));
        }


        //next state hadling
        if (nextState == "OnGround")
        {
            numOfWallJumps = 0;
            dashesRemaining = dashes;
            EventManager.singleton.AddEvent(new ChangedGroundstatemsg(gameObject, true));
        }
        if (nextState == "OnWall")
        {
            numOfWallJumps++;
            EventManager.singleton.AddEvent(new ChangedWallstatemsg(gameObject, true));
        }
        if (nextState == "Dash")
        {
            EventManager.singleton.AddEvent(new Dashmsg(gameObject));
        }

        Debug.Log("State Changed to " + nextState);

        currentState = nextState;
    }

    //--------------------------------States updates--------------------------------------------\

    void AnyStateUpdate(FrameInput frim)
    {
        isGrounded = rb.IsTouching(cfGround);
        isWallLeft = rb.IsTouching(cfWallL);
        isWallRight = rb.IsTouching(cfWallR);

        if (!frim.DashButton) {
            dashButtonReleased = true;
        }

        if (frim.DashButton && dashButtonReleased && !isDashing && dashesRemaining > 0) {
            // Save the dash direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            dashDirection = mousePos - transform.position;
            dashDirection.Normalize();

            isDashing = true;
            dashButtonReleased = false;
            dashesRemaining--;

            StartCoroutine(StateMutexWait(dashTime));
        }

    }

    void OnGroundUpdate(FrameInput frim)
    {
        //state change
        if (!isGrounded)
        {
            StateChange("OnFly");
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
            model.localScale = new Vector3(1, 1, 1); //right
        }
        else if (frim.LeftButton)
        {
            horizontal = -1;
            model.localScale = new Vector3(-1f, 1, 1); //left
        }

        if (frim.UpButton)
        {
            rb.velocity = new Vector2(horizontal * speed, jumpPower);
            EventManager.singleton.AddEvent(new Jumpmsg(gameObject));
            StateChange("OnFly");
            return;
        }

        if (rb.velocity.x != 0 && horizontal == 0) //if it was moving previous frame and stopped in this
        {
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(gameObject, false));
        }
        else if (rb.velocity.x == 0 && horizontal != 0) //if it was stopped previous frame and moving in this
        {
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(gameObject, true));
        }

        rb.velocity = new Vector2(horizontal * speed, 0); //sets jump
    }

    void OnWallUpdate(FrameInput frim)
    {
        //shooting
        if (frim.ShootButton)
        {
            EventManager.singleton.AddEvent(new playerShootGunmsg(0));
        }

        if (!isGrounded && !isWallLeft && !isWallRight) //is not touching anything
        {
            StateChange("OnFly");
        }
        else if (isGrounded)
        {
            StateChange("OnGround");
            return;
        }

        if (isWallLeft)
        {
            model.localScale = new Vector3(1, 1, 1); //right
        }

        if (isWallRight)
        {
            model.localScale = new Vector3(-1f, 1, 1); //left
        }

        if (frim.RightButton && isWallLeft)
        {
            rb.velocity = new Vector2(speed, 0);
            StateChange("OnFly");
            return;
        }

        if (frim.LeftButton && isWallRight)
        {
            rb.velocity = new Vector2(-speed, 0);
            StateChange("OnFly");
            return;
        }

        if (mJump != frim.UpButton) //checks that jump button was released before
        {
            if (frim.UpButton && numOfWallJumps <= maxWallJumps)
            {
                if (isWallLeft)
                {
                    rb.velocity = new Vector2(wallSideJumpX, wallSideJumpY * 2); //right
                    EventManager.singleton.AddEvent(new Jumpmsg(gameObject));
                    StateChange("OnWallJumping");
                    StartCoroutine(StateMutexWait(wallJumpTime));
                    return;
                }

                else if (isWallRight)
                {
                    rb.velocity = new Vector2(-wallSideJumpX, wallSideJumpY * 2); //left
                    EventManager.singleton.AddEvent(new Jumpmsg(gameObject));
                    StateChange("OnWallJumping");
                    StartCoroutine(StateMutexWait(wallJumpTime));
                    return;
                }

            }
        }

        //one wall contact see
        if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(0, -wallSlidingSpeed);

        }
        else
        {
            rb.velocity = ApplyGravity(frim.UpButton);
        }


    }

    void OnWallJumpingUpdate(FrameInput frim)
    {
        rb.velocity = ApplyGravity(frim.UpButton, rb.velocity);
        // StateMutex will block this state from changing to OnFly until the wall jump time is over
        StateChange("OnFly");
    }

    void OnFlyUpdate(FrameInput frim)
    {
        //shooting
        if (frim.ShootButton)
        {
            EventManager.singleton.AddEvent(new playerShootGunmsg(0));
        }

        if (!isGrounded && (isWallLeft || isWallRight))
        {
            StateChange("OnWall");
            return;
        }
        else if (isGrounded)
        {
            StateChange("OnGround");
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
            model.localScale = new Vector3(1, 1, 1); //right
        }
        else if (frim.LeftButton)
        {
            horizontal = -1;
            model.localScale = new Vector3(-1f, 1, 1); //left
        }

        float movement = horizontal * gladingSpeed;

        rb.velocity = ApplyGravity(frim.UpButton, new Vector2(movement, rb.velocity.y));

    }

    //animation handling
    void OnDashUpdate()
    {
        rb.velocity = dashDirection * dashSpeed;

        // Wait for the StateMutexWait to be over
        if (!StateMutex) {
            // Set velocity to almost zero
            rb.velocity = dashDirection * dashSpeed * 0.2f;
            isDashing = false;
            StateChange("OnFly");
        }
    }

    void OnDeathUpdate()
    {

    }

    // ----------------------Helper Functions--------------------------

    IEnumerator StateMutexWait(float waitTime)
    {
        StateMutex = true;
        yield return new WaitForSeconds(waitTime);
        StateMutex = false;
    }

    void DebugPrintInput(FrameInput frim)
    {
        Debug.Log(frim.RightButton + ", " + frim.LeftButton + ", " + frim.UpButton + ", "
            + frim.DownButton + ", " + frim.DashButton + ", " + frim.ShootButton + ", " + frim.armRotation);
    }

    Vector2 ApplyGravity(bool tightJump)
    {
        return ApplyGravity(tightJump, rb.velocity);
    }

    Vector2 ApplyGravity(bool tightJump, Vector2 vector)
    {
        float gravityEffect = tightJump ? gravity / tightJumpScale : gravity;
        return new Vector2(vector.x, vector.y - gravityEffect);
    }

}
