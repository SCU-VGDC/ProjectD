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
    public State currentState;

    [HideInInspector]
    public bool isGrounded, isWallRight, isWallLeft;

    [HideInInspector]
    public bool mJump;
    [HideInInspector]
    public int numOfWallJumps = 3;

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
    [HideInInspector]
    public int dashesRemaining;

    [HideInInspector]
    public bool isDashing = false;
    [HideInInspector]
    public bool dashButtonReleased = true;
    [HideInInspector]
    public Vector3 dashDirection;

    //public ContactFilter2D contactFilter;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        model = transform.Find("BodyModel");
        cfGround.SetNormalAngle(85, 95); //perpendicular to ground
        cfWallL.SetNormalAngle(0, 5); //perpendicular to left wall
        cfWallR.SetNormalAngle(175, 185); //perpendicual to right wall

        // The order of these states matter- the first state where CanStart is true will be the one that starts
        states = new State[] {
            new Dashing(this),
            new Grounded(this),
            new OnWall(this),
            new WallJumping(this),
            new Airborne(this)
        };

        // Set the initial state to be Grounded
        currentState = states[1];
    }

    private void FixedUpdate()
    {
        FrameInput thisFrame = InputHandler(); // this can be chnaged into AI

        isGrounded = rb.IsTouching(cfGround);
        isWallLeft = rb.IsTouching(cfWallL);
        isWallRight = rb.IsTouching(cfWallR);

        UpdateArmPos(thisFrame);
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

    void UpdateArmPos(FrameInput frim) {
        arm.localRotation = Quaternion.Euler(0, 0, frim.armRotation);
    }

    //--------------------------------States changes--------------------------------------------\

    private void StateHandling(FrameInput frim) {
        foreach (State state in states) {
            if (state.CanStart(frim)) {
                ChangeState(state);
                break;
            }
        }

        Debug.Log("Current State: " + currentState.name);
        currentState.Update(frim);

        if (frim.UpButton != mJump) //jump button state
        {
            mJump = frim.UpButton;
        }
    }

    // Passing in the state name will search for the matching State object and change to that
    public void ChangeState(string stateName) {
        foreach (State state in states) {
            if (state.name == stateName) {
                ChangeState(state);
                return;
            }
        }

        Debug.LogError("State " + stateName + " not found");
    }

    public void ChangeState(State nextState)
    {
        if (nextState == currentState) //beacuse same state
        {
            return;
        }

        currentState = nextState;
        currentState.Start();
        Debug.Log("State Changed to " + nextState);

    }

    // ----------------------Helper Functions--------------------------

    void DebugPrintInput(FrameInput frim)
    {
        Debug.Log(frim.RightButton + ", " + frim.LeftButton + ", " + frim.UpButton + ", "
            + frim.DownButton + ", " + frim.DashButton + ", " + frim.ShootButton + ", " + frim.armRotation);
    }

    public Vector2 ApplyGravity(bool tightJump)
    {
        return ApplyGravity(tightJump, rb.velocity);
    }

    public Vector2 ApplyGravity(bool tightJump, Vector2 vector)
    {
        float gravityEffect = tightJump ? gravity / tightJumpScale : gravity;
        return new Vector2(vector.x, vector.y - gravityEffect);
    }

}
