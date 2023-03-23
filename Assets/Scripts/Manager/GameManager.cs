using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

public class GameManager : MonoBehaviour
{
    public PersistentData persistentData;

    // runs before start
    void Awake()
    {
        persistentData =  new Backend.PersistentData().Initialize();
    }

    void OnApplicationQuit()
    {
        persistentData.Save();
    }
}
