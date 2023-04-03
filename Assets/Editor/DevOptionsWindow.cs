using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dev;


public class DevOptionsWindow : EditorWindow
{
    DeveloperOptions devOptions;
    
    [MenuItem("Window/Developer Options %#d")]
    public static void ShowWindow()
    {
        GetWindow<DevOptionsWindow>("Developer Options");
    }

    void OnEnable() 
    {
        devOptions = new DeveloperOptions().Initialize();
    }

    void OnGUI()
    {
        devOptions.SavePersistentData = GUILayout.Toggle(devOptions.SavePersistentData, "Save persistent data to disk");
    }
}
