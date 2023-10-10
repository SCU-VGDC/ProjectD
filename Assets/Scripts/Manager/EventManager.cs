using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager singleton;

    Queue<msg> EventQueue;
    public int QueueSize;

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
        Sender.GetComponent<ActorShooting>().Shoot(target);
        Sender.GetComponent<AudioManager>().PlaySound(specShootSound);
        Sender.GetComponent<AnimatorManager>().SetAnim("Attack");
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
        Sender.GetComponent<AnimatorManager>().SetAnim("TakeDamage");
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

        GameManager.inst.playerMovement.currentDashes++;
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
        }
        else
        {
            Sender.GetComponent<AnimatorManager>().SetAnim("Moving", false);
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