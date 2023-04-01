using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

public class GameManager : MonoBehaviour
{
    public PersistentData persistentData;
    [HideInInspector] public int currentPersistentDataSetPropIndex = 0;
    [HideInInspector] public string currentPersistentDataSetPropValue = "value";

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
