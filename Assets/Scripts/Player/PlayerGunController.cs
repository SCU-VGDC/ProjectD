using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerGunController : MonoBehaviour
{
    private AnimatorManager handAnimManager;
    public string currentGun;
    private float TimeSpent;

    [Header("Guns Avalibilty")]
    public bool pistolUnlocked;
    public bool sniperUnlocked;
    public bool shotgunUnlocked;

    [Header("Guns Stats")]
    public Pistol pistol;
    public Sniper sniper;
    public Shotgun shotgun;


    public void Start()
    {
        currentGun = Pistol.name;
        handAnimManager = GetComponent<PlayerMov_FSM>().arm.GetComponent<AnimatorManager>();
    }

    public void Update()
    {
        TimeSpent += Time.deltaTime;
    }

    public bool AskedToChangeGun(int gunType) // 0 - pistol, 1 - sniper, 2 - shotgun
    {
        if(gunType == 0 && pistolUnlocked)
        {
            currentGun = Pistol.name;

            return true;
        }

        if (gunType == 1 && sniperUnlocked)
        {
            currentGun = Sniper.name;

            return true;
        }

        if (gunType == 2 && shotgunUnlocked)
        {
            currentGun = Shotgun.name;

            return true;
        }

        return false;
    }

    public void AskedToShoot(int shootType) // 0 - normal, 1 - magic
    {
        switch (currentGun) {
            case Pistol.name:
                if (shootType == 0 && TimeSpent > pistol.normalFireRate)
                {
                    EventManager.singleton.AddEvent(new shootmsg(gameObject, pistol, true));

                    handAnimManager.SetAnim(pistol.normalShotAnimation);
                    TimeSpent = 0;
                }
                else if (shootType == 1 && TimeSpent > pistol.magicFireRate)
                {
                    EventManager.singleton.AddEvent(new shootmsg(gameObject, pistol, false));

                    handAnimManager.SetAnim(pistol.magicShotAnimation);
                    TimeSpent = 0;
                }
                
                break;
            case Shotgun.name:
                if (shootType == 0 && TimeSpent > shotgun.normalFireRate)
                {
                    EventManager.singleton.AddEvent(new shootmsg(gameObject, shotgun, true));

                    handAnimManager.SetAnim(shotgun.normalShotAnimation);
                    TimeSpent = 0;
                }
                else if (shootType == 1 && TimeSpent > shotgun.magicFireRate)
                {
                    EventManager.singleton.AddEvent(new shootmsg(gameObject, shotgun, false));

                    handAnimManager.SetAnim(shotgun.magicShotAnimation);
                    TimeSpent = 0;
                }
                break;

            case Sniper.name:
                if (shootType == 0 && TimeSpent > sniper.normalFireRate)
                {
                    EventManager.singleton.AddEvent(new shootmsg(gameObject, sniper, true));

                    handAnimManager.SetAnim(sniper.normalShotAnimation);
                    TimeSpent = 0;
                }
                else if (shootType == 1 && TimeSpent > sniper.magicFireRate)
                {
                    EventManager.singleton.AddEvent(new shootmsg(gameObject, sniper, false));

                    handAnimManager.SetAnim(sniper.magicShotAnimation);
                    TimeSpent = 0;
                }
                break;
        }
    }
}

[System.Serializable]
public class Gun
{
    public float normalFireRate;
    public float magicFireRate;
    public int normalDamage;
    public int magicDamage;
    public string normalShotType;
    public string magicShotType;
    public string normalShotAnimation;
    public string magicShotAnimation;
    public string normalSound;
    public string magicSound;
}

[System.Serializable]
public class Pistol : Gun
{
    public const string name = "Pistol";

    public int normalNumOfMaxRicochets;
    public int magicNumOfMaxRicochets;
}

[System.Serializable]
public class Shotgun : Gun
{
    public const string name = "Shotgun";

    public float normalRange;
    public float magicRange;
    public float normalDegrees; 
    public float magicDegrees;
    public int normalNumOfRays;
    public int magicNumOfRays;
}

[System.Serializable]
public class Sniper : Gun
{
    public const string name = "Sniper";

    public int normalNumOfMaxPenetrations;
    public float magicGrenadeForce;
}