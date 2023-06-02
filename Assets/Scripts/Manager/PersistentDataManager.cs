using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

public class PersistentDataManager : MonoBehaviour
{
    /// <summary>
    /// Variable to be used to access the PersistantDataManager in the current scene.
    /// </summary>
    public static PersistentDataManager inst;

    public PersistentData persistentData;
    [HideInInspector] public int currentPersistentDataSetPropIndex = 0;
    [HideInInspector] public string currentPersistentDataSetPropValue = "value";

    // runs before start
    void Awake()
    {
        //Basic singleton code.
        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(this);
        }

        persistentData =  new Backend.PersistentData().Initialize();

        DontDestroyOnLoad(this);
    }

    void OnApplicationQuit()
    {
        persistentData.Save();
    }
}
