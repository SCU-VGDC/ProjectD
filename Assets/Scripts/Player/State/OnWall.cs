using System.Net.Security;
using UnityEngine;

class OnWall : PlayerState
{
    public OnWall(PlayerMov_FSM pm) : base(pm) { }
    private Vector2 lastWallNormal;
    private Vector2 currentWallNormal;

    public override string name { get { return "OnWall"; } }

    public override void Start() {
        base.Start();
        pm.numOfWallJumps++;
        EventManager.singleton.AddEvent(new ChangedWallstatemsg(pm.gameObject, true));
        //TODO    
        pm.dashesRemaining = pm.dashes;
        EventManager.singleton.GetComponent<UIManager>().updateDashUI();

        lastWallNormal = currentWallNormal;
    }

    public override bool CanStart(PlayerMov_FSM.FrameInput frim)
    {
        // when infinite wall jumps are enabled, the player should not be able to repeatedly walljump on the same side
        RaycastHit2D hit = Physics2D.Raycast(pm.model.position, new Vector2(pm.isWallLeft ? -1 : 1, 0), pm.plrCollider.size.x / 2f + 0.1f, LayerMask.GetMask("Platforms"));
        currentWallNormal = hit.normal;
        if (pm.isGrounded)
        {
            lastWallNormal = Vector2.zero;
            currentWallNormal = Vector2.zero;
        }

        bool isWallNormalAcceptable = !pm.infiniteWalljump || lastWallNormal == Vector2.zero || Vector2.Angle(lastWallNormal, currentWallNormal) >= pm.minWallstickNormalAngle;
        return !frim.DownButton && pm.currentState.name == "Airborne" && !pm.isGrounded && (pm.isWallLeft || pm.isWallRight) && isWallNormalAcceptable;
    }

    public override void Update(PlayerMov_FSM.FrameInput frim) {
        base.Update(frim);

        //shooting
        pm.model.localScale = new Vector3(pm.isWallLeft ? 1 : -1, 1, 1); //right
        bool isFacingRight = (frim.armRotation < 90 && frim.armRotation > -90);
        bool facingCorrect = pm.isWallRight ^ isFacingRight;
        if (frim.ShootButton && facingCorrect)
        {
            EventManager.singleton.AddEvent(new playerShootGunmsg(0));
        }
        if (frim.ShootAltButton && facingCorrect)
        {
            EventManager.singleton.AddEvent(new playerShootGunmsg(1));
        }

        if (frim.DownButton || (pm.isWallRight && frim.LeftButton) || (pm.isWallLeft && frim.RightButton))
        {
            pm.rb.velocity = new Vector2(pm.speed * (pm.isWallLeft ? 1 : -1), 0);
            SetState("Airborne");
            return;
        }

        if (pm.mJump != frim.UpButton) //checks that jump button was released before
        {
            if (frim.UpButton && (pm.numOfWallJumps <= pm.maxWallJumps || pm.infiniteWalljump))
            {
                pm.rb.velocity = new Vector2(pm.wallSideJumpX * (pm.isWallLeft ? 1 : -1), pm.wallSideJumpY * 2); //right
                EventManager.singleton.AddEvent(new Jumpmsg(pm.gameObject));
                SetState("WallJumping");
                return;
            }
        }

        //one wall contact see
        if (pm.rb.velocity.y < 0)
            pm.rb.velocity = new Vector2(0, -pm.wallSlidingSpeed);
        else
            pm.rb.velocity = pm.ApplyGravity(frim.UpButton);
    }

    public override void Exit() {
        base.Exit();
        EventManager.singleton.AddEvent(new ChangedWallstatemsg(pm.gameObject, false));
    }
}