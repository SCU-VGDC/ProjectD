using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Dev;


namespace Backend {

    /// <summary> 
    /// A class responsible for all data that is stored between game sessions and scenes <para />
    /// <example>
    /// For example:
    /// <code>
    /// Using Backend; <para />
    /// <para />
    /// private PersistentData pd; <para />
    /// private void Start() <para />
    /// { <para />
    /// <para /> pd = GameObject.Find("Persistent Data Manager").GetComponent&lt;&#8203;PersistentDataManager&gt;().persistentData <para />
    /// } <para />
    /// </code>
    /// </example>
    /// </summary>
    [SerializeField] public class PersistentData
    {
        // data:
        /// <summary> Location of where the persistent data is saved on disk </summary>
        readonly public static string path = Application.persistentDataPath + @"\data.json";

        // you have to do this weird getter and setter thing bc of JsonUtility.ToJson
        //   more info: https://gamedev.stackexchange.com/a/178746
        // when adding a new variable to the persistent data make sure to do the weird getter and setter (instructions above)
        //   and also add to the functions CopyFrom and InitValues
        // player:
        [SerializeField] private string playerCurrentState;
        public string PlayerCurrentState { get { return this.playerCurrentState; } set { this.playerCurrentState = value; } }
        [SerializeField] private int playerHealth;
        public int PlayerHealth { get { return this.playerHealth; } set { this.playerHealth = value; } }
        [SerializeField] private int playerMaxHealth;
        public int PlayerMaxHealth { get { return this.playerMaxHealth; } set { this.playerMaxHealth = value; } }
        //[SerializeField] private int playerNumDashes;
        //public int PlayerNumDashes { get { return this.playerNumDashes; } set { this.playerNumDashes = value; } }
        [SerializeField] private int playerMaximumDashes;
        public int PlayerMaximumDashes { get { return this.playerMaximumDashes; } set { this.playerMaximumDashes = value; } }
        
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
        /// <summary> Adds the amount to the player health if the amount is less than max player health </summary>
        public void AddPlayerHealth(int amount) 
        {
            if (playerHealth < playerMaxHealth || (amount < 0 && playerHealth > 0)) 
            {
                playerHealth += amount;
            }
        }

        /*public void AddPlayerNumDashes(int amount) 
        {
            playerNumDashes += amount;
        }*/

        /// <summary> Saves the current persistent data as JSON and writes it to disk </summary>
        public void Save()
        {
            #if (UNITY_EDITOR)
                DeveloperOptions devOptions = new DeveloperOptions().Load();

                if (devOptions != null && !devOptions.SavePersistentData)
                {
                    return;
                }

            #endif
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
            playerMaxHealth = newPersistentData.PlayerMaxHealth;
            //playerNumDashes = newPersistentData.PlayerNumDashes;
            playerMaximumDashes = newPersistentData.PlayerMaximumDashes;
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
            playerMaxHealth = 20;
            //playerNumDashes = 3;
            playerMaximumDashes = 3;
            playerCurrentGun = null;
            playerUnlockedGuns = new List<string>();
            playerCurrentCheckpoint = null;
            playerDecisions = new Dictionary<string, string>();
        }
    }

}
