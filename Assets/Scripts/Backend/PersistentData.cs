using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Backend {

    /// <summary>
    /// To use, access from Game Manager like so:
    /// pd = GameObject.Find("Game Manager").GetComponent<GameManager>().persistentData;
    /// </summary>
    [SerializeField] public class PersistentData
    {
        // data:
        /// <summary> Location of where the persistent data is saved on disk </summary>
        readonly public static string path = Application.persistentDataPath + @"\data.json";

        // you have to do this weird getter and setter thing bc of JsonUtility.ToJson
        //   more info: https://gamedev.stackexchange.com/a/178746
        // player:
        [SerializeField] private string playerCurrentState;
        public string PlayerCurrentState { get { return this.playerCurrentState; } set { this.playerCurrentState = value; } }
        [SerializeField] private int playerHealth;
        public int PlayerHealth { get { return this.playerHealth; } set { this.playerHealth = value; } }
        [SerializeField] private int playerNumDashes;
        public int PlayerNumDashes { get { return this.playerNumDashes; } set { this.playerNumDashes = value; } }
        
        // guns:
        [SerializeField] private string playerCurrentGun;
        public string PlayerCurrentGun { get { return this.playerCurrentGun; } set { this.playerCurrentGun = value; } }
        [SerializeField] private List<string> playerUnlockedGuns;
        public List<string> PlayerUnlockedGuns { get { return this.playerUnlockedGuns; } set { this.playerUnlockedGuns = value; } }

        // level:
        [SerializeField] private GameObject playerCurrentCheckpoint;
        public GameObject PlayerCurrentCheckpoint { get { return this.playerCurrentCheckpoint; } set { this.playerCurrentCheckpoint = value; } }

        // decisions:
        [SerializeField] private Dictionary<string, string> playerDecisions;
        public Dictionary<string, string> PlayerDecisions { get { return this.playerDecisions; } set { this.playerDecisions = value; } }

        /// <summary> Creates an PersistentData object from data saved to disk or default values if nothing is saved in disk </summary>
        public PersistentData Initialize() 
        {
            PersistentData perviousPersistentData = Load();

            // load previous data if exists, else initialize values
            if (perviousPersistentData != null) 
            {
                CopyFrom(perviousPersistentData);
            } 
            else
            {
                InitValues();
            }

            return this;
        }


        // player functions:
        public void AddPlayerHealth(int amount) 
        {
            playerHealth += amount;
        }

        public void AddPlayerNumDashes(int amount) 
        {
            playerNumDashes += amount;
        }

        /// <summary> Saves the current persistent data as JSON and writes it to disk </summary>
        public void Save()
        {
            string json = JsonUtility.ToJson(this, true);

            File.WriteAllText(path, json);
        }

        /// <summary> Loads previously saved JSON if exists. If doesn't exist, returns null </summary>
        public PersistentData Load()
        {
            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);

            PersistentData persistentData = JsonUtility.FromJson<PersistentData>(json);
            return persistentData;
        }

        /// <summary> Copies over all attributes of newPersistentData to this PersistentData </summary>
        public void CopyFrom(PersistentData newPersistentData)
        {
            playerCurrentState = newPersistentData.PlayerCurrentState;
            playerHealth = newPersistentData.PlayerHealth;
            playerNumDashes = newPersistentData.PlayerNumDashes;
            playerCurrentGun = newPersistentData.PlayerCurrentGun;
            playerUnlockedGuns = newPersistentData.PlayerUnlockedGuns;
            playerCurrentCheckpoint = newPersistentData.PlayerCurrentCheckpoint;
            playerDecisions = newPersistentData.PlayerDecisions;
        }

        /// <summary> Initializes the values. ONLY call if you want to reset all of the games persistent data </summary>
        public void InitValues()
        {
            playerCurrentState = "alive";
            playerHealth = 20;
            playerNumDashes = 3;
            playerCurrentGun = null;
            playerUnlockedGuns = new List<string>();
            playerCurrentCheckpoint = null;
            playerDecisions = new Dictionary<string, string>();
        }
    }

}
