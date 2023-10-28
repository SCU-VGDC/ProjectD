using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Semisolid : MonoBehaviour
{
    private TilemapCollider2D tilemapCollider2D;
    private GameObject player;
    private bool falling;
    private float delay = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        tilemapCollider2D = gameObject.GetComponent<TilemapCollider2D>();
        player = GameObject.Find("Player");
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
                player.layer = LayerMask.NameToLayer("Default");
                Invoke("FallCooldown", delay);
            }
        }
    }

    private void FallCooldown()
    {
        falling = false;
        player.layer = LayerMask.NameToLayer("Player");
    }
}
