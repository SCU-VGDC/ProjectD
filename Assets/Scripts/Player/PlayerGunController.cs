using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    public Gun currentGun;
    private float TimeSpent;

    [Header("Guns Avalibilty")]
    public bool isPistol;
    public bool isSniper;
    public bool isShotgun;

    [Header("Guns Stats")]
    public Gun pistol;
    public Gun sniper;
    public Gun shotgun;


    public void Start()
    {
        currentGun = shotgun;
    }

    public void Update()
    {
        TimeSpent = TimeSpent + Time.deltaTime;
    }

    public void AskedToChangeGun(int gunType) // 0 - pistol, 1 - sniper, 2 - shotgun
    {
        if(gunType == 0 && isPistol)
        {
            currentGun = pistol;
        }

        if (gunType == 1 && isSniper)
        {
            currentGun = sniper;
        }

        if (gunType == 2 && isShotgun)
        {
            currentGun = shotgun;
        }
    }

    public void AskedToShoot(int shootType) // 0 - normal, 1 - magic
    {
        if(TimeSpent > currentGun.firerate)
        {
            if (shootType == 0) {
                EventManager.singleton.AddEvent(new shootmsg(gameObject, null, "GunShot", currentGun.projectileNormal));

            } else if (shootType == 1) {
                EventManager.singleton.AddEvent(new shootmsg(gameObject, null, "GunShot", currentGun.projectileMagic));
            }

            //HAND shit solution???
            GetComponent<PlayerMov_FSM>().arm.GetComponent<AnimatorManager>().SetAnim(currentGun.shotAnimation);
            TimeSpent = 0;
        }
    }

    public void AskedToDash()
    {
        
    }

}

[System.Serializable]
public class Gun
{
    public float firerate;
    public string projectileNormal;
    public string projectileMagic;
    public string shotAnimation;
    public string soundNormal;
    public string soundMagic;
}
