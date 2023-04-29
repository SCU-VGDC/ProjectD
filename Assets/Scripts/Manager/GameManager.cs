using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// This variable acts as a way to access any information contained in the GameManager.
    /// It will always be equal to the GameManager in the active scene, if it exists.
    /// </summary>
    public static GameManager inst;

    public GameObject enemyBlood;

    public ObjectPool<GameObject> bloodPool;

    private void Awake()
    {
        //Basic singleton pattern. Make sure there is only ever 1 GameManager in the scene and updates inst accordingly.

        //If no GameManager assigned...
        if (inst == null)
        {
            //This is our GameManager
            inst = this;
        }
        //If there is a GameManager assigned, and it's not this
        else if (inst != this)
        {
            //Then this shouldn't exist, destroy it.
            Destroy(this);
        }

        bloodPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(enemyBlood),
            actionOnGet: (obj) => {
                obj.SetActive(true);
            },
            actionOnRelease: (obj) => {
                obj.SetActive(false);
                obj.GetComponent<Blood_Behavior>().Deinitialize();
            },
            collectionCheck: true,
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: 10000,
            maxSize: 50000
        );
    }
}
