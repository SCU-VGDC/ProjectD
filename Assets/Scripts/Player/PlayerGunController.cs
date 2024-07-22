using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    private AnimatorManager handAnimManager;
    public Gun currentGun;
    private float TimeSpent;

    [Header("Guns Avalibilty")]
    public bool pistolUnlocked;
    public bool sniperUnlocked;
    public bool shotgunUnlocked;

    [Header("Guns Stats")]
    public Gun pistol;
    public Gun sniper;
    public Gun shotgun;


    public void Start()
    {
        currentGun = pistol;
        handAnimManager = GetComponent<PlayerMov_FSM>().arm.GetComponent<AnimatorManager>();
    }

    public void Update()
    {
        TimeSpent = TimeSpent + Time.deltaTime;
    }

    public void AskedToChangeGun(int gunType) // 0 - pistol, 1 - sniper, 2 - shotgun
    {
        if(gunType == 0 && pistolUnlocked)
        {
            currentGun = pistol;
        }

        if (gunType == 1 && shotgunUnlocked)
        {
            currentGun = sniper;
        }

        if (gunType == 2 && shotgunUnlocked)
        {
            currentGun = shotgun;
        }
    }

    public void AskedToShoot(int shootType) // 0 - normal, 1 - magic
    {
        if(TimeSpent > currentGun.fireRate)
        {
            if (shootType == 0) {
                EventManager.singleton.AddEvent(new shootmsg(gameObject, null, currentGun.soundNormal, currentGun.shotTypeNormal));
            } else if (shootType == 1) {
                EventManager.singleton.AddEvent(new shootmsg(gameObject, null,  currentGun.soundMagic, currentGun.shotTypeMagic));
            }

            handAnimManager.SetAnim(currentGun.shotAnimation);
            TimeSpent = 0;
        }
    }
}

[System.Serializable]
public class Gun
{
    public float fireRate;
    public string shotTypeNormal;
    public string shotTypeMagic;
    public string shotAnimation;
    public string soundNormal;
    public string soundMagic;
}
