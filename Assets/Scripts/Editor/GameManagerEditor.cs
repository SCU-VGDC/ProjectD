using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override bool RequiresConstantRepaint() => true; // needed to repaint the currentPersistentDataPropsContent

    GameManager gameManager;
    GUIStyle defaultGUIStyle = GUIStyle.none;
 
    void OnEnable() 
    {
        gameManager = (GameManager) target;

        defaultGUIStyle.normal.textColor = Color.white;
        defaultGUIStyle.wordWrap = true;
        defaultGUIStyle.richText = true;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // current persistent data properties
        GUIContent currentPersistentDataPropsContent = new GUIContent("<b>Current Persistent Data Properties:</b>\n");
        if (Application.isPlaying)
        {
            foreach(var prop in gameManager.persistentData.GetType().GetProperties()) 
            {
                currentPersistentDataPropsContent.text += $"{prop.Name}: {toStringProp(prop)}\n";
            }
        } 
        else 
        {
            currentPersistentDataPropsContent.text += "Editor is not in play mode";
        }

        GUILayout.Label(currentPersistentDataPropsContent, defaultGUIStyle);
        GUILayout.Label("");

        // reset current persistent data properties
        if(GUILayout.Button("Reset all current persistent data values"))
        {
            if (Application.isPlaying)
            {
                gameManager.persistentData.InitValues();
            } 
            else
            {
                Debug.LogAssertion("Cannot reset all current persistent data values since the editor is not in play mode", this);
            }
        }
        GUILayout.Label("");

        // persistent data location
        GUIContent persistentDataLocationContent = new GUIContent("<b>Persistent Data Location:</b>\n" + Application.persistentDataPath);

        GUILayout.Label(persistentDataLocationContent, defaultGUIStyle);
        GUILayout.Label("");
        
        // current data on disk and last saved data
        GUIContent currentDataOnDiskContent = new GUIContent("<b>Data Saved on Disk:</b>\n");
        GUIContent lastSavedDataContent = new GUIContent("<b>Data Last Saved:</b>\n");
        string dataPath = Backend.PersistentData.path;
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            string lastModified = File.GetLastWriteTime(dataPath).ToString("MM/dd/yyyy hh:mm tt");

            currentDataOnDiskContent.text += json;
            lastSavedDataContent.text += lastModified;
        }
        else
        {
            currentDataOnDiskContent.text += "No data saved to disk";
            lastSavedDataContent.text += "No data saved to disk";
        }

        GUILayout.Label(currentDataOnDiskContent, defaultGUIStyle);
        GUILayout.Label("");
        GUILayout.Label(lastSavedDataContent, defaultGUIStyle);
        GUILayout.Label("");

        // delete data on disk
        if(GUILayout.Button("Delete data on disk"))
        {
            File.Delete(dataPath);
        }
    }

    string toStringProp(System.Reflection.PropertyInfo prop)
    {
        object propValue = prop.GetValue(gameManager.persistentData, null);

        if (propValue == null)
        {
            return "null";
        }

        switch (propValue)
        {
            case string:
                return $"\"{propValue}\"";
            case List<string>:
                return $"[{String.Join(", ", (propValue as List<string>).Select(value => $"\"{value}\""))}]";

            case Dictionary<string, string>:
                if ((propValue as Dictionary<string, string>).Count == 0)
                {
                    return "{}";
                }
                
                string dictionaryStr = "{";
                foreach (KeyValuePair<string, string> entry in propValue as Dictionary<string, string>)
                {
                    dictionaryStr += $"\n  \"{entry.Key}\": \"{entry.Value}\",";
                }
                dictionaryStr += "\n}";

                return dictionaryStr;

            case UnityEngine.GameObject:
                return $"{(propValue as UnityEngine.GameObject).name} (instanceID: {(propValue as UnityEngine.GameObject).GetInstanceID()})";

            default:
                return propValue.ToString();
        }
    }
}
