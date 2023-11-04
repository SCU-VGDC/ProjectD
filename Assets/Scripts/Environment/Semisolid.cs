using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Semisolid : MonoBehaviour
{
    private float delay = 0.25f;
    private PlatformEffector2D platformEffector2D;
    private LayerMask currentLayer;

    public static Semisolid SemiSolidTilemapInst;

    // Start is called before the first frame update
    void Start()
    {
        SemiSolidTilemapInst = this;
        platformEffector2D = GetComponent<PlatformEffector2D>();
        currentLayer = platformEffector2D.colliderMask;
    }

    // Update is called once per frame
    public void FallSemiSolid()
    {
        platformEffector2D.colliderMask = currentLayer & ~LayerMask.GetMask("Player");
        Invoke("FallCooldown", delay);
    }

    private void FallCooldown()
    {
        platformEffector2D.colliderMask = currentLayer | LayerMask.GetMask("Player");
    }
}
