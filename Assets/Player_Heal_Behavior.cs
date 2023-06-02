using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

public class Player_Heal_Behavior : MonoBehaviour
{
    private PersistentData pd;

    // Start is called before the first frame update
    void Start()
    {
        pd = PersistentDataManager.inst.persistentData;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("Collide!");
        if(collider.gameObject.layer == 14)
        {
            //heal player
            pd.AddPlayerHealth(1);
            //Debug.Log("Heal!");
        }
    }
}
