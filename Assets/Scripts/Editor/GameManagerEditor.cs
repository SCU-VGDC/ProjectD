using UnityEngine;
using UnityEditor;
using Backend;
using System.IO;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUIStyle defaultGUIStyle = GUIStyle.none;
        defaultGUIStyle.normal.textColor = Color.white;
        defaultGUIStyle.wordWrap = true;
        defaultGUIStyle.richText = true;

        // persistent data location
        GUIContent persistentDataLocationContent = new GUIContent("<b>Persistent Data Location:</b>\n" + Application.persistentDataPath);

        GUILayout.Label(persistentDataLocationContent, defaultGUIStyle);
        GUILayout.Label("");
        
        // current data on disk
        GUIContent currentDataOnDiskContent;
        string dataPath = Application.persistentDataPath + @"\data.json";
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);

            currentDataOnDiskContent = new GUIContent("<b>Data Saved on Disk:</b>\n" + json);
        }
        else
        {
            currentDataOnDiskContent = new GUIContent("<b>Data Saved on Disk:</b>\nNo data saved to disk");
        }

        GUILayout.Label(currentDataOnDiskContent, defaultGUIStyle);
        GUILayout.Label("");

        // delete data on disk
        if(GUILayout.Button("Delete data on disk"))
        {
            File.Delete(dataPath);
        }
    }
}
