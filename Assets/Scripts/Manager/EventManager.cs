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
        if(QueueSize != 0)
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

    public shootmsg(GameObject m_shooter, Transform m_target = null, string m_specShootSound = "GunShot") : base(m_shooter)
    {
        target = m_target;
        specShootSound = m_specShootSound;
    }

    public override void Run()
    {
        if (Sender.GetComponent<ActorShooting>().raycastToggle)
        {
            Sender.GetComponent<ActorShooting>().ShootRaycast(0);
        }
        else {
            Sender.GetComponent<ActorShooting>().Shoot(target);
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
        EventManager.singleton.AddEvent(new applyDamagemsg(Sender, target.GetComponent<ActorHealth>(), damage));
        Sender.GetComponent<AudioManager>().PlaySound("MeleeAttack");
        Sender.GetComponent<AnimatorManager>().SetAnim("MeleeAttack");

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
        target.ApplyDamage(damage);

        target.GetComponent<AudioManager>().PlaySound("TakeDamage");
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

        if (target.tag != "Player")
        {
            int bloodCount = 50;
            Debug.Log("Tried to spawn blood!");
            for (int i = 0; i < bloodCount; i++)
            {
                Vector2 rand = UnityEngine.Random.insideUnitCircle;
                GameObject droplet;
                GameManager.inst.bloodPool.Get(out droplet);
                droplet.transform.position = target.transform.position + new Vector3(rand.x, rand.y, 0);
            }
        }
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

        pgc.AskedToShoot();
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
        Sender.GetComponent<PlayerGunController>().AskedToChangeGun(GunType);
        Debug.Log("Change Sound");
        Debug.Log("Update Gun Sound");
    }
}

public class playerDiedmsg : msg
{


    public playerDiedmsg() : base(GameManager.inst.player)
    {
    }

    public override void Run()
    {
        //TODO
    }
}

public class playerRespawnmsg : msg
{


    public playerRespawnmsg() : base(GameManager.inst.player)
    {
    }

    public override void Run()
    {
        //TODO
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
            Debug.Log("Orb Sound");
        }
        if (is_Dash_orb == true)
        {
            Debug.Log("Orb Thorugh Sound");
        }

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
    }
}

public class Jumpmsg : msg
{

    public Jumpmsg(GameObject m_sender) : base(m_sender)
    {
        
    }

    public override void Run()
    {
        Sender.GetComponent<AudioManager>().PlaySound("Jump");
        Sender.GetComponent<AnimatorManager>().SetAnim("Jump");
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
        //TODO
        Sender.GetComponent<AudioManager>().PlaySound("Death");
        Sender.GetComponent<AnimatorManager>().SetAnim("Death");
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
        Sender.GetComponent<AudioManager>().PlaySound("Checkpoint");
        Sender.GetComponent<AnimatorManager>().SetAnim("Checkpoint");
    }
}

public class overrideMovement : msg
{
    public overrideMovement(GameObject endTransition, PlayerMov_FSM.FrameInput frameInput) : base(endTransition)
    {

    }

    public override void Run()
    {
        // TODO
        
    }
}
