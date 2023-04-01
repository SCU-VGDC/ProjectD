using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using Backend;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override bool RequiresConstantRepaint() => true; // needed to repaint the currentPersistentDataPropsContent

    GameManager gameManager;
    string dataPath;
    bool dataExistsOnDisk;
    GUIStyle defaultGUIStyle = GUIStyle.none;
 
    void OnEnable() 
    {
        gameManager = (GameManager) target;

        dataPath = Backend.PersistentData.path;

        defaultGUIStyle.normal.textColor = Color.white;
        defaultGUIStyle.wordWrap = true;
        defaultGUIStyle.richText = true;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        dataExistsOnDisk = File.Exists(dataPath);

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
        if (dataExistsOnDisk)
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

        // save and load data on disk
        GUILayout.BeginHorizontal();
        if (!Application.isPlaying)
        {
            GUI.backgroundColor = Color.gray; // changes background color of button
        }
        if(GUILayout.Button("Save data to disk"))
        {
            if (Application.isPlaying)
            {
                gameManager.persistentData.Save();
            }
            else
            {
                Debug.LogAssertion("Cannot save current persistent data to disk since the editor is not in play mode", this);
            }
        }
        GUI.backgroundColor = Color.white;

        if (!(dataExistsOnDisk && Application.isPlaying))
        {
            GUI.backgroundColor = Color.gray;
        }
        if(GUILayout.Button("Load data from disk"))
        {
            if (dataExistsOnDisk && Application.isPlaying)
            {
                PersistentData persistentData = gameManager.persistentData.Load();
                gameManager.persistentData.CopyFrom(persistentData);
            }
            else
            {
                Debug.LogAssertion("Cannot load persistent data from disk since the editor is not in play mode and there is no data on disk", this);
            }
        }
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;

        // delete data on disk
        if(GUILayout.Button("Delete data on disk"))
        {
            File.Delete(dataPath);
        }
    }

    string toStringProp(System.Reflection.PropertyInfo prop)
    {
        object propValue = prop.GetValue(gameManager.persistentData, null);

        if (propValue == null || propValue.Equals(null))
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
