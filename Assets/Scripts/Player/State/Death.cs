using UnityEngine;

class Death : PlayerState
{
    public Death(PlayerMov_FSM pm) : base(pm) { }

    public override string name { get { return "Death"; } }

    public override void Start()
    {
        base.Start();

        // stop the clock!
    }

    public override void Update(PlayerMov_FSM.FrameInput frim)
    {
        base.Update(frim);

        if (frim.RespawnButton)
        {
            EventManager.singleton.AddEvent(new playerRespawnmsg());
            SetState("Grounded");
        }
    }
}