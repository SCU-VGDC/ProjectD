using System;
using System.Linq;
using UnityEngine;

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

        public bool Gun1Button;
        public bool Gun2Button;
        public bool Gun3Button;

        public float armRotation;
    }

    private bool StateMutex;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Transform model;
    [NonSerialized]
    public BoxCollider2D plrCollider;

    private ContactFilter2D cfGround;
    private ContactFilter2D cfWallR;
    private ContactFilter2D cfWallL;

    // Make sure to update this array when adding new states
    private PlayerState[] states = new PlayerState[6];
    public PlayerState currentState;

    [HideInInspector]
    public bool isGrounded, isWallRight, isWallLeft;
    public bool infiniteWalljump = false;

    // minimum angle between two walls' normals when walljumping, only applies when infiniteWallJump is true
    [NonSerialized]
    public int minWallstickNormalAngle = 90;

    [HideInInspector]
    public bool mJump;
    [HideInInspector]
    public int numOfWallJumps = 3;

    public float speed;
    public float jumpPower;
    public float gravity; //I am so sorry
    public float maxFallSpeed; // Max falling speed, someone help him he is in danger
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

    public bool overloadMovement;
    public FrameInput overloadedInput;

    //public ContactFilter2D contactFilter;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        plrCollider = GetComponent<BoxCollider2D>();
        model = transform.Find("BodyModel");
        cfGround.SetNormalAngle(85, 95); //perpendicular to ground
        cfWallL.SetNormalAngle(0, 5); //perpendicular to left wall
        cfWallR.SetNormalAngle(175, 185); //perpendicual to right wall

        // The order of these states matter- the first state where CanStart is true will be the one that starts
        states = new PlayerState[] {
            new Death(this),
            new Dashing(this),
            new Grounded(this),
            new OnWall(this),
            new WallJumping(this),
            new Airborne(this),
        };

        // Set the initial state to be Grounded
        currentState = GetState<Grounded>();
    }

    private void FixedUpdate()
    {
        FrameInput thisFrame = InputHandler(); // this can be chnaged into AI

        if(overloadMovement)
        {
            thisFrame = overloadedInput;
        }

        isGrounded = rb.IsTouching(cfGround);
        isWallLeft = rb.IsTouching(cfWallL);
        isWallRight = rb.IsTouching(cfWallR);


        StateHandling(thisFrame);

        UpdateArmPos(thisFrame);
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
        thisFrame.ShootAltButton = false;
        thisFrame.InteractButton = false;
        thisFrame.RespawnButton = false;
        thisFrame.Gun1Button = false;
        thisFrame.Gun2Button = false;
        thisFrame.Gun3Button = false;


        thisFrame.armRotation = 0;

        thisFrame.UpButton = Input.GetButton("Up");
        thisFrame.DownButton = Input.GetButton("Crouch");
        thisFrame.RightButton = Input.GetButton("Right");
        thisFrame.LeftButton = Input.GetButton("Left");

        thisFrame.DashButton = Input.GetButton("Dash");
        thisFrame.ShootButton = Input.GetButton("Shoot");
        thisFrame.ShootAltButton = Input.GetButton("AltShoot");

        thisFrame.InteractButton = Input.GetButton("Interact");
        thisFrame.RespawnButton = Input.GetButton("Interact");

        thisFrame.Gun1Button = Input.GetButton("Gun1");
        thisFrame.Gun2Button = Input.GetButton("Gun2");
        thisFrame.Gun3Button = Input.GetButton("Gun3");

        Vector3 Mouse_Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = Mouse_Pos - transform.position;
        float rotation_z = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        thisFrame.armRotation = rotation_z;

        return thisFrame;
    }

    void UpdateArmPos(FrameInput frim) 
    {

        arm.localRotation = Quaternion.Euler(0, 0, frim.armRotation);
        if(frim.armRotation > 90 || frim.armRotation < -90)
        {
            arm.GetChild(0).localScale = new Vector3(1f, -1f, 1f); 
        }
        else
        {
            arm.GetChild(0).localScale = new Vector3(1f, 1f, 1f);
        }
    }

    //--------------------------------States changes--------------------------------------------\

    private void StateHandling(FrameInput frim) 
    {
        foreach (PlayerState state in states) 
        {
            if (state.CanStart(frim))
            {
                SetState(state);
                break;
            }
        }

        //LOOK OVER IT, I am sending messages no matter which state, is this okay??
        if(frim.InteractButton)
        {
            EventManager.singleton.AddEvent(new interactmsg(gameObject));
        }

        if (frim.Gun1Button)
        {
            EventManager.singleton.AddEvent(new playerChangeGunmsg(0));
        }
        if (frim.Gun2Button)
        {
            EventManager.singleton.AddEvent(new playerChangeGunmsg(1));
        }
        if (frim.Gun3Button)
        {
            EventManager.singleton.AddEvent(new playerChangeGunmsg(2));
        }

        //ÊÎÑÒÛËÜ
        if (Dialogue_Manager.DialogueInAction != null)
        {
            if (frim.ShootButton && !Dialogue_Manager.DialogueInAction.HaveSpawnedOptions)
            {
                Dialogue_Manager.DialogueInAction.PerformAction(0);
            }
            return;
        }

        currentState.Update(frim);

        if (frim.UpButton != mJump) //jump button state
        {
            mJump = frim.UpButton;
        }
    }

    // Passing in the state name will search for the matching State object and change to that
    public void SetState(string stateName) {
        foreach (PlayerState state in states) {
            if (state.name == stateName) {
                SetState(state);
                return;
            }
        }

        Debug.LogError("State " + stateName + " not found");
    }

    public void SetState(PlayerState nextState)
    {
        if (nextState == currentState) //beacuse same state
            return;

        currentState.Exit();
        currentState = nextState;
        currentState.Start();
        Debug.Log("State Changed to " + nextState);
    }

    public T GetState<T>() where T:PlayerState
    {
        return (T)states.First(state => typeof(T) == state.GetType());
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
        float newY = vector.y - gravityEffect;
        if (newY < -maxFallSpeed)
        {
            newY = -maxFallSpeed;
        }
        return new Vector2(vector.x, newY);
    }

}
