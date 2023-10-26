using UnityEngine;

class Grounded : State {
    public Grounded(PlayerMov_FSM playerMovement) : base(playerMovement) { }

    public override string stateName { get { return "Grounded"; } }

    public override void Start() {
        EventManager.singleton.AddEvent(new ChangedGroundstatemsg(playerMovement.gameObject, false));
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) {
        //state change
        if (!playerMovement.isGrounded)
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
            playerMovement.model.localScale = new Vector3(1, 1, 1); //right
        }
        else if (frim.LeftButton)
        {
            horizontal = -1;
            playerMovement.model.localScale = new Vector3(-1f, 1, 1); //left
        }

        if (frim.UpButton)
        {
            playerMovement.rb.velocity = new Vector2(horizontal * playerMovement.speed, playerMovement.jumpPower);
            EventManager.singleton.AddEvent(new Jumpmsg(playerMovement.gameObject));
            StateChange("OnFly");
            return;
        }

        if (playerMovement.rb.velocity.x != 0 && horizontal == 0) //if it was moving previous frame and stopped in this
        {
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(playerMovement.gameObject, false));
        }
        else if (playerMovement.rb.velocity.x == 0 && horizontal != 0) //if it was stopped previous frame and moving in this
        {
            EventManager.singleton.AddEvent(new ChangedMOVstatemsg(playerMovement.gameObject, true));
        }

        playerMovement.rb.velocity = new Vector2(horizontal * playerMovement.speed, 0); //sets jump
    }
}