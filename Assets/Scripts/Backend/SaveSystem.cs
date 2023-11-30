using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem singleton;

    public int Version = 1;
    public LevelState LastUpdatedInGameLS;

    //Main Functions

    public LevelState GetLevelData()
    {
        LevelState LS = new LevelState();

        LS.SaveVersion = Version;
        LS.LevelName = SceneManager.GetActiveScene().name;

        LS.MaxDashes = 0; //where we would store those??

        LS.MaxHealth = GameManager.inst.playerHealth.maxHealth;

        LS.pistol = GameManager.inst.player.GetComponent<PlayerGunController>().isPistol ? 1 : 0;
        LS.sniper = GameManager.inst.player.GetComponent<PlayerGunController>().isSniper ? 1 : 0;
        LS.shotgun = GameManager.inst.player.GetComponent<PlayerGunController>().isShotgun ? 1 : 0;

        foreach (Checkpoint cp in FindObjectsOfType<Checkpoint>())
        {
            if(cp.LastActivated == true)
            {
                LS.checkpointId = cp.CheckPointId;
                break;
            }
        }

        LS.ClearedArenas = new List<int>();
        foreach (RoomManager rm in FindObjectsOfType<RoomManager>())
        {
            if (rm.isCompleted == true)
            {
                LS.ClearedArenas.Add(rm.ArenaId);
            }
        }

        LS.WanderingEnemies = new List<BasicEnemyState>();
        foreach (Base_Enemy enem in FindObjectsOfType<Base_Enemy>())
        {
            if (enem.transform.parent)
            {
                if (enem.transform.parent.name.Contains("Wave"))
                {
                    continue;
                }
            }

            if(enem.gameObject.layer != 9) //enemies layer
            {
                continue;
            }

            BasicEnemyState bes = new BasicEnemyState();

            bes.EnemyLocation = enem.transform.position;
            bes.PrefabName = enem.PrefabName;

            LS.WanderingEnemies.Add(bes);
        }

        return LS;
    }

    public void CreateWorld(LevelState givenLevel)
    {
        if(givenLevel.SaveVersion != Version)
        {
            Debug.LogError("SaveVersion Is Not Compatible");
            return;
        }

        GameManager.inst.playerHealth.maxHealth = givenLevel.MaxHealth;
        //GameManager.inst.playerHealth.MaxDashes = MaxDashes; //where to get info??

        GameManager.inst.player.GetComponent<PlayerGunController>().isPistol = (givenLevel.pistol > 0) ? true : false;
        GameManager.inst.player.GetComponent<PlayerGunController>().isSniper = (givenLevel.sniper > 0) ? true : false;
        GameManager.inst.player.GetComponent<PlayerGunController>().isShotgun = (givenLevel.shotgun > 0) ? true : false;

        foreach (Checkpoint cp in FindObjectsOfType<Checkpoint>())
        {
            if (cp.CheckPointId == givenLevel.checkpointId)
            {
                GameManager.inst.player.transform.position = cp.transform.position;
                cp.LastActivated = true;
                break;
            }
        }

        foreach (RoomManager rm in FindObjectsOfType<RoomManager>())
        {
            if(givenLevel.ClearedArenas.Contains(rm.ArenaId))
            {
                rm.isCompleted = true;
            }
        }

        foreach (Base_Enemy enem in FindObjectsOfType<Base_Enemy>())
        {
            if (enem.transform.parent)
            {
                if (enem.transform.parent.name.Contains("Wave"))
                {
                    continue;
                }
            }

            if (enem.gameObject.layer == 9)
            {
                Destroy(enem.gameObject);
            }
        }

        foreach (BasicEnemyState enem in givenLevel.WanderingEnemies)
        {
            SpawnEnemy(enem);
        }

        LastUpdatedInGameLS = givenLevel;
    }

    public void SaveData()
    {
        string path = Application.persistentDataPath + @"\saves.json";

        if(File.Exists(path) == false)
        {
            File.Create(path);
        }

        string[] AllSaves = File.ReadAllLines(path);

        List<LevelState> LSlist = new List<LevelState>();

        string currentName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < AllSaves.Length; i++) //if save rewrite
        {
            LSlist.Add(JsonUtility.FromJson<LevelState>(AllSaves[i]));
            if (LSlist[i].LevelName == currentName)
            {
                AllSaves[i] = JsonUtility.ToJson(GetLevelData());
                File.WriteAllLines(path, AllSaves);
                return;
            }
        }

        //if no save
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(JsonUtility.ToJson(GetLevelData()));
        writer.Close();
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/saves.json";

        if (File.Exists(path) == false)
        {
            File.Create(path);
        }

        string[] AllSaves = File.ReadAllLines(path);

        List<LevelState> LSlist = new List<LevelState>();

        string currentName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < AllSaves.Length; i++) //if save rewrite
        {
            LSlist.Add(JsonUtility.FromJson<LevelState>(AllSaves[i]));
            if (LSlist[i].LevelName == currentName)
            {
                CreateWorld(LSlist[i]);
                return;
            }
        }

        Debug.LogError("NO SAVE FOUND");
    }

    public void TempResetAllLevelsData()
    {
        string path = Application.persistentDataPath + @"\saves.json";

        FileStream fileStream = File.Open(path, FileMode.Open);
        fileStream.SetLength(0);
        fileStream.Close();
    }

    public void Start() //used for testing
    {
        TempResetAllLevelsData();
        //LastUpdatedInGameLS = GetLevelData();
        //Debug.Log(Application.persistentDataPath + "/saves.json");
        //SaveData();
        //LoadData();
        //CreateWorld(LastUpdatedInGameLS);
    }

    public void Awake()
    {
        singleton = this;
    }

    void SpawnEnemy(BasicEnemyState givenEnemy)
    {
        GameObject enemy = Instantiate(Resources.Load("Prefabs/EnemyPrefabs/" + givenEnemy.PrefabName)) as GameObject;
        enemy.transform.position = givenEnemy.EnemyLocation;
    }
}


[Serializable]
public class LevelState
{
    public int SaveVersion;
    public string LevelName;

    //Player
    public int MaxDashes;
    public int MaxHealth;

    //Player Guns
    public int pistol;
    public int sniper;
    public int shotgun;

    //Current CheckPoint
    public int checkpointId;

    //World State
    public List<int> ClearedArenas;
    public List<BasicEnemyState> WanderingEnemies;
}

[Serializable]
public class BasicEnemyState
{
    public Vector2 EnemyLocation;
    public string PrefabName;
}


