using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehavior : MonoBehaviour
{
    //type of the key
    public string keyType = "";

    //animation vars
    public float bobMagnitude = 1f;
    public float bobSpeed = 1f;

    //component vars
    private Transform keyLightTrans;
    private float originalLightY;
    private Transform keySpriteTrans;
    private float originalSpriteY;

    void Start()
    {
        //get key light obj
        keyLightTrans = transform.GetChild(0);

        //get key light y
        originalLightY = keyLightTrans.position.y;

        //get key sprite obj
        keySpriteTrans = transform.GetChild(1);

        //get key sprite y
        originalSpriteY = keySpriteTrans.position.y;
    }

    void Update()
    {
        //undulate light up and down
        keyLightTrans.position = new Vector3(keyLightTrans.position.x, bobMagnitude * Mathf.Sin(Time.time * bobSpeed) + originalLightY, 0f);

        //undulate sprite up and down
        keySpriteTrans.position = new Vector3(keySpriteTrans.position.x, bobMagnitude * Mathf.Sin(Time.time * bobSpeed) + originalSpriteY, 0f);
    }
}
