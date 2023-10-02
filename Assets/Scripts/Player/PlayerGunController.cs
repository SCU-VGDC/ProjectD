using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    private int currentWeapon;
    private Gun currentGun;

    [Header("Guns Avalibilty")]
    public bool isPistol;
    public bool isDash;
    public bool isSniper;
    public bool isShotgun;

    [Header("Guns Stats")]
    public Gun pistol;
    public Gun sniper;
    public Gun shotgun;

    private ActorShooting playerShooter;

    public void Start()
    {
        playerShooter = GetComponent<ActorShooting>();
        currentWeapon = 0;
        currentGun = pistol;
    }

    public void AskedToChangeGun(int gunType) // 0 - pistol, 1 - sniper, 2 - shotgun
    {
        if(gunType == 0)
        {
            currentWeapon = 0;
            currentGun = pistol;
        }

        if (gunType == 1)
        {
            currentWeapon = 0;
            currentGun = pistol;
        }

        if (gunType == 2)
        {
            currentWeapon = 0;
            currentGun = pistol;
        }
    }

    public void AskedToShoot(int fireType) // 0 - normal, 1 - magic
    {
        if(fireType == 0)
        {
            playerShooter.SetBullet(currentGun.projectileNormal);         
        }
        if (fireType == 1)
        {
            playerShooter.SetBullet(currentGun.projectileMagic);

        }

        playerShooter.Shoot();
    }

    public void AskedToDash()
    {
        
    }

}

public class Gun
{
    public int firerate;
    public GameObject projectileNormal;
    public GameObject projectileMagic;
}
