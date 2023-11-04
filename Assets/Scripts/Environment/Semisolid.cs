using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Semisolid : MonoBehaviour
{
    private GameObject player;
    private bool falling;
    private float delay = 0.25f;
    private PlatformEffector2D platformEffector2D;
    private LayerMask currentLayer;

    // Start is called before the first frame update
    void Start()
    {
        platformEffector2D = GetComponent<PlatformEffector2D>();
        player = GameObject.Find("Player");
        currentLayer = platformEffector2D.colliderMask;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!falling)
            {
                Debug.Log("Attempted to Fall Through Semisolid");
                falling = true;
                platformEffector2D.colliderMask = currentLayer & ~LayerMask.GetMask("Player");
                Invoke("FallCooldown", delay);
            }
        }
    }

    private void FallCooldown()
    {
        falling = false;
        platformEffector2D.colliderMask = currentLayer | LayerMask.GetMask("Player");
    }
}
