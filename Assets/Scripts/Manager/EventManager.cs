using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager singleton;

    Queue<msg> EventQueue;
    public int QueueSize;

    public Interactable LastInteractable;

    private void Awake()
    {
        singleton = this;
        EventQueue = new Queue<msg>();
    }

    private void Update()
    {
        while(QueueSize != 0)
        {
            msg Popped = EventQueue.Dequeue();
            ResolveEvent(Popped);
        }
    }

    private void ResolveEvent(msg NextPop)
    {
        QueueSize--;

        NextPop.Run();
    }

    public void AddEvent(msg sendMessage)
    {
        QueueSize++;
        EventQueue.Enqueue(sendMessage);
    }

}

public class msg
{
    protected GameObject Sender;

    public msg(GameObject m_sender)
    {
        Sender = m_sender;
    }
    public virtual void Run()
    {

    }
}

public class shootmsg : msg
{
    public Transform target;
    public string specShootSound;
    public string typeShot;

    public shootmsg(GameObject m_shooter, Transform m_target = null, string m_specShootSound = "GunShot", string m_typeShot = null) : base(m_shooter)
    {
        target = m_target;
        specShootSound = m_specShootSound;
        typeShot = m_typeShot;
    }

    public override void Run()
    {
        switch (typeShot) {
            // player shots
            case "PistolShot":
                Sender.GetComponent<ActorShooting>().ShootRaycastSingleBullet(10, 0, 0);

                break;
            case "PistolShotRicochet":
                Sender.GetComponent<ActorShooting>().ShootRaycastSingleBullet(10, 0, 3);

                break;
            case "ShotgunShot":
                Sender.GetComponent<ActorShooting>().ShootRaycastSpreadBullets(1, 5.0f, 90.0f, 10);

                break;
            // enemy shots
            default:
                Sender.GetComponent<ActorShooting>().Shoot(target);

                break;
        }

        Sender.GetComponent<AudioManager>().PlaySound(specShootSound);
        Sender.GetComponent<AnimatorManager>().SetAnim("Attack");
    }
}

public class meleeDamagemsg : msg
{
    public Transform target;
    public int damage;

    public meleeDamagemsg(GameObject m_shooter, Transform m_target = null, int m_damage = 0) : base(m_shooter)
    {
        target = m_target;
        damage = m_damage;
    }

    public override void Run()
    {
        if (target)
        {
            if(target.GetComponent<ActorHealth>())
            {
                if (target.GetComponent<ActorHealth>().died)
                {
                    return;
                }
            }          
        }

        Sender.GetComponent<AudioManager>().PlaySound("MeleeAttack");
        Sender.GetComponent<AnimatorManager>().SetAnim("MeleeAttack");

        EventManager.singleton.AddEvent(new applyDamagemsg(Sender, target.GetComponent<ActorHealth>(), damage));

        if(Sender.GetComponent<Base_Enemy>())
        {
            if(Sender.GetComponent<Base_Enemy>().destroyOnContact)
            {
                UnityEngine.Object.Destroy(Sender);
            }
        }
    }
}

public class applyDamagemsg : msg
{
    public ActorHealth target;
    public int damage;

    public applyDamagemsg(GameObject m_shooter, ActorHealth m_target, int m_damage) : base(m_shooter)
    {
        target = m_target;
        damage = m_damage;
    }

    public override void Run()
    {

        if (target.died)
        {
            return;
        }

        //Save initial health for Blood calculation.
        int initialHealth = target.currentHealth;

        target.ApplyDamage(damage);

        target.GetComponent<AudioManager>().PlaySound("TakeDamage");

        //The TakeDamage animation sometimes overrides the Death animation! This fixes that

        target.GetComponent<AnimatorManager>().SetAnim("TakeDamage");


        

        if (target.transform.childCount != 0)
        {
            if (target.transform.GetChild(0).GetComponent<MoveGearPlatforms>()) //cheking for movegears
            {

                MoveGearPlatforms mvg = target.transform.GetChild(0).GetComponent<MoveGearPlatforms>();
                mvg.ChangeMove();
                return;

            }
        }


        //Only spawn blood for Enemies, not Player
        if (target.tag != "Player")
        {
            //TODO: Perhaps make blood coefficient be set on a per-enemy basis? i.e. bosses spawn less to make more challenging.
            float bloodCoeff = 1.5f;

            //Spawn blood droplets based on the *effective* damage dealth.
            //i.e. if you deal 10 damage but the enemy has 3 health, spawn blood based on the 3 health you actually take away.
            int bloodCount = Mathf.FloorToInt(Mathf.Min(Mathf.Abs(damage), Mathf.Abs(initialHealth)) * bloodCoeff);
            Debug.LogFormat("Spawning {0} blood particles.", bloodCount);
            for (int i = 0; i < bloodCount; i++)
            {
                Vector2 rand = UnityEngine.Random.insideUnitCircle;
                GameObject droplet;
                GameManager.inst.bloodPool.Get(out droplet);
                droplet.transform.position = target.transform.position + new Vector3(rand.x, rand.y, 0);
            }
        }
        else
        {
            EventManager.singleton.GetComponent<UIManager>().updateHealthUI();
        }

        //Not sure why this was here twice? Commented out -Gabe
        //target.GetComponent<AudioManager>().PlaySound("TakeDamage");
        //target.GetComponent<AnimatorManager>().SetAnim("TakeDamage");
    }
}

public class healActormsg : msg
{
    public ActorHealth target;
    public int heal;

    public healActormsg(GameObject m_shooter, ActorHealth m_target, int m_heal) : base(m_shooter)
    {
        target = m_target;
        heal = m_heal;
    }

    public override void Run()
    {
        target.ApplyDamage(-heal);
        EventManager.singleton.GetComponent<UIManager>().updateHealthUI();
    }
}

public class playerShootGunmsg : msg
{
    int shootType;

    public playerShootGunmsg(int m_shootType) : base(GameManager.inst.player)
    {
        shootType = m_shootType;
    }

    public override void Run()
    {
        PlayerGunController pgc = Sender.GetComponent<PlayerGunController>();
        ActorShooting ac = Sender.GetComponent<ActorShooting>();
        AudioManager ad = Sender.GetComponent<AudioManager>();

        if (shootType == 0)
        {
            ac.SetBullet(pgc.currentGun.projectileNormal);
            ad.ChangeSoundInDict("GunShot", pgc.currentGun.soundNormal);
        }
        if (shootType == 1)
        {
            ac.SetBullet(pgc.currentGun.projectileMagic);
            ad.ChangeSoundInDict("GunShot", pgc.currentGun.soundMagic);
        }

        pgc.AskedToShoot(shootType);
    }
}

public class playerChangeGunmsg : msg
{
    int GunType;

    public playerChangeGunmsg(int m_GunType) : base(GameManager.inst.player)
    {
        GunType = m_GunType;
    }

    public override void Run()
    {
        AudioManager ad = Sender.GetComponent<AudioManager>();
    
        Sender.GetComponent<PlayerGunController>().AskedToChangeGun(GunType);
        ad.PlaySound("GunChangeSound");
    }
}

public class playerDiedmsg : msg
{


    public playerDiedmsg() : base(GameManager.inst.player)
    {
    }

    public override void Run()
    {
        Sender.GetComponent<AudioManager>().PlaySound("Death");
        Sender.GetComponent<AnimatorManager>().SetAnim("Death", true);

        GameManager.inst.playerMovement.SetState("Death");
    }
}

public class playerRespawnmsg : msg
{


    public playerRespawnmsg() : base(GameManager.inst.player)
    {
    }

    public override void Run()
    {
        foreach (RoomManager rm in GameObject.FindObjectsOfType<RoomManager>())
        {
            rm.RespawnEnemiesInside();
        }

        SaveSystem.singleton.LoadData();
        SaveSystem.singleton.CreateWorld(SaveSystem.singleton.LastUpdatedInGameLS);

        Sender.GetComponent<AudioManager>().PlaySound("Respawn");
        Sender.GetComponent<AnimatorManager>().SetAnim("Death", false);       
        GameManager.inst.playerHealth.currentHealth = GameManager.inst.playerHealth.maxHealth;
        GameManager.inst.playerHealth.died = false;
        EventManager.singleton.GetComponent<UIManager>().updateHealthUI();
    }
}

public class OrbPickUpmsg : msg
{
    bool is_Dash_orb;

    public OrbPickUpmsg(GameObject m_orb, bool m_is_Dash_orb) : base(m_orb)
    {
        is_Dash_orb = m_is_Dash_orb;
    }

    public override void Run()
    {
        
        if(is_Dash_orb == false)
        {
            Sender.GetComponent<AudioManager>().PlaySound("OrbNormal");
        }
        if (is_Dash_orb == true)
        {
            Sender.GetComponent<AudioManager>().PlaySound("OrbDash");
        }

        //TODO
        GameManager.inst.playerMovement.dashesRemaining++;
    }
}

public class Dashmsg : msg
{

    public Dashmsg(GameObject m_sender) : base(m_sender)
    {

    }

    public override void Run()
    {
        Sender.GetComponent<AudioManager>().PlaySound("Dash");
        Sender.GetComponent<AnimatorManager>().SetAnim("Dash");

        Sender.GetComponent<PlayerMov_FSM>().arm.GetComponent<AnimatorManager>().SetAnim("Dash");

        EventManager.singleton.GetComponent<UIManager>().updateDashUI();
    }
}



public class Jumpmsg : msg
{
    bool isjumping;
    public Jumpmsg(GameObject m_sender,bool m_is_jumping=true) : base(m_sender)
    {
        isjumping=m_is_jumping;
    }

    public override void Run()
    {
        if(isjumping==true)// defatul to true so need to add it in for anything.
        {
        Sender.GetComponent<AudioManager>().PlaySound("Jump");
        Sender.GetComponent<AnimatorManager>().SetAnim("Jump");
        }
        else if(isjumping==false)
        {
        Sender.GetComponent<AnimatorManager>().SetAnim("Jump",true);//this is to reset the trigger and to send it but it needs a truth value.
        }
    }
}

public class ChangedMOVstatemsg : msg
{
    bool is_moving;

    public ChangedMOVstatemsg(GameObject m_sender, bool m_is_moving) : base(m_sender)
    {
        is_moving = m_is_moving;
    }

    public override void Run()
    {
        if(is_moving)
        {
            Sender.GetComponent<AudioManager>().PlaySound("Moving");
            Sender.GetComponent<AnimatorManager>().SetAnim("Moving", true);
            //HAND
            if(Sender.GetComponent<PlayerMov_FSM>())
            {
                Sender.GetComponent<PlayerMov_FSM>().arm.GetComponent<AnimatorManager>().SetAnim("Moving", true);
            }

        }
        else
        {
            Sender.GetComponent<AnimatorManager>().SetAnim("Moving", false);
            //HAND
            if (Sender.GetComponent<PlayerMov_FSM>())
            {
                Sender.GetComponent<PlayerMov_FSM>().arm.GetComponent<AnimatorManager>().SetAnim("Moving", false);
            }
        }
    }
}

public class ChangedMOVBackstatemsg : msg
{
    bool is_moving;

    public ChangedMOVBackstatemsg(GameObject m_sender, bool m_is_moving) : base(m_sender)
    {
        is_moving = m_is_moving;
    }

    public override void Run()
    {
        if (is_moving)
        {
            Sender.GetComponent<AudioManager>().PlaySound("Moving");
            Sender.GetComponent<AnimatorManager>().SetAnim("BackMoving", true);
        }
        else
        {
            Sender.GetComponent<AnimatorManager>().SetAnim("BackMoving", false);
        }
    }
}

public class ChangedGroundstatemsg : msg
{
    bool is_on_ground;

    public ChangedGroundstatemsg(GameObject m_sender, bool m_is_on_ground) : base(m_sender)
    {
        is_on_ground = m_is_on_ground;
    }

    public override void Run()
    {
        if (is_on_ground)
        {
            Sender.GetComponent<AudioManager>().PlaySound("Landed");
            Sender.GetComponent<AnimatorManager>().SetAnim("Ground", true);
        }
        else
        {
            Sender.GetComponent<AnimatorManager>().SetAnim("Ground", false);
        }
    }
}

public class ChangedWallstatemsg : msg
{
    bool is_on_wall;

    public ChangedWallstatemsg(GameObject m_sender, bool m_is_on_wall) : base(m_sender)
    {
        is_on_wall = m_is_on_wall;
    }

    public override void Run()
    {
        if (is_on_wall)
        {
            Sender.GetComponent<AudioManager>().PlaySound("OnWall");
            Sender.GetComponent<AnimatorManager>().SetAnim("Wall", true);
        }
        else
        {
            Sender.GetComponent<AnimatorManager>().SetAnim("Wall", false);
        }
    }
}

public class interactmsg : msg
{
    public interactmsg(GameObject m_sender) : base(m_sender)
    {

    }

    public override void Run()
    {
        if(EventManager.singleton.LastInteractable != null)
        {
            EventManager.singleton.LastInteractable.Activation();
            EventManager.singleton.GetComponent<UIManager>().updateinteractUI(false);
        }
    }
}

public class actorDiedmsg : msg
{
    public actorDiedmsg(GameObject m_shooter) : base(m_shooter)
    {

    }

    public override void Run()
    {
        Sender.GetComponent<AudioManager>().PlaySound("Death");
        Sender.GetComponent<AnimatorManager>().SetAnim("Death", true);

        if (Sender.transform.parent)
        {
            if (Sender.transform.parent.tag == "ResourcePrefab")
            {
                GameObject.Destroy(Sender.transform.parent.gameObject, 1f);
            }
        }
        else
        {

            GameObject.Destroy(Sender, 1f);
        }

    }
}

public class newCheckPointmsg : msg
{
    public newCheckPointmsg(GameObject m_shooter) : base(m_shooter)
    {

    }

    public override void Run()
    {
        SaveSystem.singleton.SaveData();

        foreach (Checkpoint cp in UnityEngine.Object.FindObjectsOfType<Checkpoint>())
        {
            cp.GetComponent<AnimatorManager>().SetAnim("isActivated", false);
        }

        Sender.GetComponent<AudioManager>().PlaySound("Checkpoint");
        Sender.GetComponent<AnimatorManager>().SetAnim("isActivated", true);
    }
}

public class playerPressedCrouch : msg
{
    public playerPressedCrouch(GameObject m_shooter) : base(m_shooter)
    {

    }

    public override void Run()
    {
        Semisolid.SemiSolidTilemapInst.FallSemiSolid();
    }
}

public class overrideMovement : msg
{
    bool start;
    PlayerMov_FSM.FrameInput overrideInputGiven;

    public overrideMovement(GameObject endTransition, PlayerMov_FSM.FrameInput m_frameInput, bool m_start) : base(endTransition)
    {
        start = m_start;
        overrideInputGiven = m_frameInput;
    }

    public override void Run()
    {
        GameManager.inst.playerMovement.overloadMovement = start;
        GameManager.inst.playerMovement.overloadedInput = overrideInputGiven;
    }
}

public class changeDoor : msg
{
    bool close;

    public changeDoor(GameObject door, bool m_close) : base(door)
    {
        close = m_close;
    }

    public override void Run()
    {
        Sender.GetComponent<AnimatorManager>().SetAnim("Close", close);
        Sender.GetComponent<BoxCollider2D>().enabled = close;
    }
}
