using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using Backend;

[CustomEditor(typeof(PersistentDataManager))]
public class PersistentDataManagerEditor : Editor
{
    public override bool RequiresConstantRepaint() => true; // needed to repaint the currentPersistentDataPropsContent

    PersistentDataManager pdManager;
    string dataPath;
    bool dataExistsOnDisk;
    GUIStyle defaultGUIStyle = GUIStyle.none;
 
    void OnEnable() 
    {
        pdManager = (PersistentDataManager) target;

        dataPath = Backend.PersistentData.path;

        defaultGUIStyle.normal.textColor = Color.white;
        defaultGUIStyle.wordWrap = true;
        defaultGUIStyle.richText = true;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("");

        dataExistsOnDisk = File.Exists(dataPath);

        // current persistent data properties
        GUIContent currentPersistentDataPropsContent = new GUIContent("<b>Current Persistent Data Properties:</b>\n");
        if (Application.isPlaying)
        {
            foreach(var prop in pdManager.persistentData.GetType().GetProperties()) 
            {
                currentPersistentDataPropsContent.text += $"{prop.Name}: {ToStringProp(prop)}\n";
            }
        } 
        else 
        {
            currentPersistentDataPropsContent.text += "Editor is not in play mode";
        }

        GUILayout.Label(currentPersistentDataPropsContent, defaultGUIStyle);
        GUILayout.Label("");

        // set a current persistent data property
        GUIContent currentPersistentDataSetProp = new GUIContent("<b>Set a Current Persistent Data Property</b>");
        if (Application.isPlaying)
        {
            GUILayout.Label(currentPersistentDataSetProp, defaultGUIStyle);
            GUILayout.BeginHorizontal();

            System.Reflection.PropertyInfo[] properties = pdManager.persistentData.GetType().GetProperties();
            String[] propertyOptions = properties
                .Where(prop => prop.PropertyType.IsPrimitive 
                    || prop.PropertyType.IsValueType 
                    || (prop.PropertyType == typeof(string)))
                        .Select(prop => prop.Name).ToArray<string>();
            
            pdManager.currentPersistentDataSetPropIndex = EditorGUILayout.Popup(pdManager.currentPersistentDataSetPropIndex, propertyOptions);
            pdManager.currentPersistentDataSetPropValue = EditorGUILayout.TextField(pdManager.currentPersistentDataSetPropValue); 

            if (GUILayout.Button("Set"))
            {
                var convertedValue = ConvertValueToProp(pdManager.currentPersistentDataSetPropValue, properties[pdManager.currentPersistentDataSetPropIndex]);

                if (convertedValue.canConvertValueToProp)
                {
                    properties[pdManager.currentPersistentDataSetPropIndex].SetValue(pdManager.persistentData, convertedValue.value);
                }
                else
                {
                    Debug.LogAssertion($"Failed to convert value of {pdManager.currentPersistentDataSetPropValue} ({pdManager.currentPersistentDataSetPropValue.GetType().ToString()}) to the the property, {properties[pdManager.currentPersistentDataSetPropIndex].Name}, of type {properties[pdManager.currentPersistentDataSetPropIndex].GetValue(pdManager.persistentData, null).GetType().ToString()}", this);
                }
            }

            GUILayout.EndHorizontal();
        }
        else 
        {
            currentPersistentDataSetProp.text += "\nEditor is not in play mode";
            GUILayout.Label(currentPersistentDataSetProp, defaultGUIStyle);
        }

        GUILayout.Label("");


        // reset current persistent data properties
        if (!Application.isPlaying)
        {
            GUI.backgroundColor = Color.gray; // changes background color of button
        }
        if(GUILayout.Button("Reset all current persistent data values"))
        {
            if (Application.isPlaying)
            {
                pdManager.persistentData.InitValues();
            } 
            else
            {
                Debug.LogAssertion("Cannot reset all current persistent data values since the editor is not in play mode", this);
            }
        }
        GUI.backgroundColor = Color.white;
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
                pdManager.persistentData.Save();
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
                PersistentData persistentData = pdManager.persistentData.Load();
                pdManager.persistentData.CopyFrom(persistentData);
            }
            else
            {
                Debug.LogAssertion("Cannot load persistent data from disk since the editor is not in play mode and there is no data on disk", this);
            }
        }
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;

        // delete data on disk
        if (!dataExistsOnDisk)
        {
            GUI.backgroundColor = Color.gray;
        }
        if(GUILayout.Button("Delete data on disk"))
        {
            if (dataExistsOnDisk)
            {
                File.Delete(dataPath);
            }
            else
            {
                Debug.LogAssertion("Cannot delete persistent data on disk since there is no data on disk", this);
            }
        }
        GUI.backgroundColor = Color.gray;
    }

    string ToStringProp(System.Reflection.PropertyInfo prop)
    {
        object propValue = prop.GetValue(pdManager.persistentData, null);

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

    /// <summary> Returns a bool that represents if the value can be converted to the prop and the object if the prop can be converted </summary>
    (bool canConvertValueToProp, object value) ConvertValueToProp(string value, System.Reflection.PropertyInfo prop)
    {
        object propValue = prop.GetValue(pdManager.persistentData, null);

        switch (propValue)
        {
            case int:
                int convertedValueInt;
                return (int.TryParse(value, out convertedValueInt), convertedValueInt);
            
            case float:
                float convertedValueFloat;
                return (float.TryParse(value, out convertedValueFloat), convertedValueFloat);

            case double:
                double convertedValueDouble;
                return (double.TryParse(value, out convertedValueDouble), convertedValueDouble);

            case char:
                char convertedValueChar;
                return (char.TryParse(value, out convertedValueChar), convertedValueChar);

            case bool:
                bool convertedValueBool;
                return (bool.TryParse(value, out convertedValueBool), convertedValueBool);

            case string:
                return (true, value);

            default:
                return (false, null);

        }
    }
}
