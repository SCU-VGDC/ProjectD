//using UnityEditor.Tilemaps;
using UnityEngine;

class Death : PlayerState
{
    private float timeRespawnPressed;

    public Death(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Death"; } }


    public override void Start()
    {
        base.Start();
        
        timeRespawnPressed = -1;
    }
    public override bool CanStart(PlayerMov_FSM.FrameInput frim)
    {
        return GameManager.inst.playerHealth.died;
    }

    public override void Update(PlayerMov_FSM.FrameInput frim)
    {
        base.Update(frim);

        if (pm.isGrounded)
            pm.rb.velocity = Vector2.zero;
        else
            pm.rb.velocity = pm.ApplyGravity(false);

        if (frim.RespawnButton && timeInState - timeRespawnPressed >= 1.0f)
        {
            timeRespawnPressed = timeInState;
            
            EventManager.singleton.AddEvent(new playerRespawnmsg());
            SetState("Grounded");
        }
    }
}