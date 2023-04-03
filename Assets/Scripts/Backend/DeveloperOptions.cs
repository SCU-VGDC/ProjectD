using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dev {
    [SerializeField] public class DeveloperOptions
    {
        /// <summary> Location of where the developer options is saved on disk </summary>
        readonly public static string path = Application.dataPath + @"\Editor\developerOptions.json";

        [SerializeField] private bool savePersistentData;
        public bool SavePersistentData { get { return this.savePersistentData; } set { this.savePersistentData = value; Save(); } }

        /// <summary> Creates an DeveloperOptions object from data saved to disk or default values if nothing is saved in disk </summary>
        public DeveloperOptions Initialize() 
        {
            DeveloperOptions perviouslySavedDevOptions = Load();

            // load previous data if exists, else initialize values
            if (perviouslySavedDevOptions != null) 
            {
                CopyFrom(perviouslySavedDevOptions);
            } 
            else
            {
                InitValues();
            }

            return this;
        }

        /// <summary> Saves the current developer options data as JSON and writes it to disk </summary>
        public void Save()
        {
            string json = JsonUtility.ToJson(this, true);

            File.WriteAllText(path, json);
        }

        /// <summary> Loads previously saved JSON if exists. If doesn't exist, returns null </summary>
        public DeveloperOptions Load()
        {
            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);

            DeveloperOptions persistentData = JsonUtility.FromJson<DeveloperOptions>(json);
            return persistentData;
        }

        /// <summary> Copies over all attributes of newDevOptions to this DeveloperOptions </summary>
        public void CopyFrom(DeveloperOptions newDevOptions)
        {
            savePersistentData = newDevOptions.SavePersistentData;
        }

        /// <summary> Initializes the values. ONLY call if you want to reset all of the games persistent data </summary>
        public void InitValues()
        {
            savePersistentData = true;
        }
    }
}