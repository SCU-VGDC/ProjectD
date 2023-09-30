using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using Mono.Cecil;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public class ActorShooting : MonoBehaviour
{
    
    public  Transform bulletspawn;//this puts the spawn position 
    public GameObject bulletprefab;//this creates a gameobject;
 public void shoot()
    {
        Instantiate(bulletprefab,bulletspawn.position, bulletspawn.rotation);

    }
}
