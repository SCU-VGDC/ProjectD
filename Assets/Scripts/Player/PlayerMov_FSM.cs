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
    struct FrameInput
    {
        public bool RightButton;
        public bool LeftButton;
        public bool UpButton;
        public bool DownButton;

        public bool DashButton;
        public bool ShootButton;
        public bool ShootAltButton;

        public bool RespawnButton;

        public float armRotation;
    }

    private bool StateMutex;
    private Rigidbody2D rb;
    private Transform model;

    private ContactFilter2D cfGround;
    private ContactFilter2D cfWallR;
    private ContactFilter2D cfWallL;

    private bool isGrounded, isWallRight, isWallLeft;
    private bool m_Jump;
    private int numOfWallJumps;

    public string currentState; // 1-Dash, 2-OnGround, 3-OnWall, 4-OnFly, 5-Death

    public float speed;
    public float jump_power;
    public float gravity; //I am so sorry
    public float wallSlidingSpeed;
    public float tightJumpScale;
    public float wallSideJumpX;

    public float wallSideJumpY;
    public float wallJumpTime;
    public float gladingSpeed;
    public int maxWallJumps;
    public Transform arm;

    //those changes are retarded
    public int currentDashes;
    public float dash_time;
    public bool is_dashing;


    //public ContactFilter2D contactFilter;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        model = transform.Find("BodyModel");
        cfGround.SetNormalAngle(85,95); //perpendicular to ground
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

    void DebugPrintInpput(FrameInput frim)
    {
        Debug.Log(frim.RightButton + ", " + frim.LeftButton + ", " + frim.UpButton + ", "
            + frim.DownButton + ", " + frim.DashButton + ", " + frim.ShootButton + ", " + frim.armRotation);
    }

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

    void StateHandling(FrameInput frin)
    {
        AnyStateUpdate(frin); //return if any state have changed the state

        switch (currentState)
        {
            case "Dash":
                OnDashUpdate();
                break;
            case "OnGround":
                OnGroundUpdate(frin);
                break;
            case "OnWall":
                OnWallUpdate(frin);
                break;
            case "OnWallJumping":
                OnWallJumpingUpdate(frin);
                break;
            case "OnFly":
                OnFlyUpdate(frin);
                break;
            case "Death":
                OnDeathUpdate();
                break;
            default:
                // lol
                break;
        }

        if(frin.UpButton != m_Jump) //jump button state
        {
            m_Jump = frin.UpButton;
        }
    }

    public void StateChange(string NextState)
    {
        if(NextState == currentState) //beacuse same state
        {
            return;
        }

        if(StateMutex == true) //mutex
        {
            return;
        }

        if(NextState == "OnGround")
        {
            numOfWallJumps = 0;
        }
        if(NextState == "OnWall")
        {
            numOfWallJumps++;
        }
        if(NextState == "Dash")
        {
            StartCoroutine(StateMutexWait(1f));
        }

        Debug.Log("State Changed to " + NextState);

        currentState = NextState;
    }

    void UpdateArmPos(FrameInput frim)
    {
        arm.localRotation = Quaternion.Euler(0, 0, frim.armRotation);
    }

    void AnyStateUpdate(FrameInput frim)
    {
        isGrounded = rb.IsTouching(cfGround);
        isWallLeft = rb.IsTouching(cfWallL);
        isWallRight = rb.IsTouching(cfWallR);

        if(frim.DashButton)
        {
            GameManager.inst.playerAnimation.SetAnim("Dash");
            StateChange("Dash");
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

        if(frim.ShootButton)
        {
            //GameManager.inst.playerShooting.Shoot();
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
            model.localScale = new Vector3(-1f,1,1); //left
        }
      
        if(frim.UpButton)
        {
            rb.velocity = new Vector2(horizontal * speed, jump_power);
            GameManager.inst.playerAnimation.SetAnim("Jump");
            StateChange("OnFly");
            return;
        }

        rb.velocity = new Vector2(horizontal * speed, 0); //sets jump

        //animation handling
        GameManager.inst.playerAnimation.SetAnim("Ground", true);
        GameManager.inst.playerAnimation.SetAnim("Wall", false);
        if (horizontal != 0)
        {
            GameManager.inst.playerAnimation.SetAnim("Moving", true);
        }
        else
        {
            GameManager.inst.playerAnimation.SetAnim("Moving", false);
        }
    }

    void OnWallUpdate(FrameInput frim)
    {
        //shooting
        if (frim.ShootButton)
        {
            //GameManager.inst.playerShooting.Shoot();
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
            rb.velocity = new Vector2(-speed , 0);
            StateChange("OnFly");
            return;
        }

        if (m_Jump != frim.UpButton) //checks that jump button was released before
        {
            if (frim.UpButton && numOfWallJumps <= maxWallJumps)
            {
                if (isWallLeft)
                {
                    rb.velocity = new Vector2(wallSideJumpX, wallSideJumpY * 2); //right
                    GameManager.inst.playerAnimation.SetAnim("Jump");
                    StateChange("OnWallJumping");
                    StartCoroutine(StateMutexWait(wallJumpTime));
                    return;
                }

                else if (isWallRight)
                {
                    rb.velocity = new Vector2(-wallSideJumpX, wallSideJumpY * 2); //left
                    GameManager.inst.playerAnimation.SetAnim("Jump");
                    StateChange("OnWallJumping");
                    StartCoroutine(StateMutexWait(wallJumpTime));
                    return;
                }

            }
        }

        //one wall contact see
        if (rb.velocity.y < 0) {
            rb.velocity = new Vector2(0, -wallSlidingSpeed);
            //animation handling
            GameManager.inst.playerAnimation.SetAnim("Ground", false);
            GameManager.inst.playerAnimation.SetAnim("Wall", true);
        }
        else {
            rb.velocity = ApplyGravity(frim.UpButton);
            //animation handling
            GameManager.inst.playerAnimation.SetAnim("Jump", true);
        }
            

    }

    void OnWallJumpingUpdate(FrameInput frim) {
        rb.velocity = ApplyGravity(frim.UpButton, rb.velocity);
        // StateMutex will block this state from changing to OnFly until the wall jump time is over
        StateChange("OnFly");
    }

    void OnFlyUpdate(FrameInput frim)
    {
        //shooting
        if (frim.ShootButton)
        {
           // GameManager.inst.playerShooting.Shoot();
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

        //animation handling
        GameManager.inst.playerAnimation.SetAnim("Ground", false);
        GameManager.inst.playerAnimation.SetAnim("Wall", false);
    }

    void OnDashUpdate()
    {
        StateChange("OnFly");
    }

    void OnDeathUpdate()
    {

    }

    IEnumerator StateMutexWait(float waitTime)
    {
        StateMutex = true;
        yield return new WaitForSeconds(waitTime);
        StateMutex = false;
    }

    Vector2 ApplyGravity(bool tightJump) {
        return ApplyGravity(tightJump, rb.velocity);
    }

    Vector2 ApplyGravity(bool tightJump, Vector2 vector) {
        float gravityEffect = tightJump ? gravity / tightJumpScale : gravity;
        return new Vector2(vector.x, vector.y - gravityEffect);
    }
}
