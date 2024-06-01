using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehavior : MonoBehaviour
{
    //type of the key
    public string keyType = "";

    private Transform keySpriteTrans;
    private Transform keyLightTrans;

    void Start()
    {
        //get key light obj
        keyLightTrans = transform.GetChild(0);

        //get key sprite obj
        keySpriteTrans = transform.GetChild(1);
    }

    void Update()
    {
        //undulate light up and down
        keyLightTrans.position += new Vector3(0f, Mathf.Sin(Time.deltaTime), 0f);

        //undulate sprite up and down
        keySpriteTrans.position += new Vector3(0f, Mathf.Sin(Time.deltaTime), 0f);
    }
}
