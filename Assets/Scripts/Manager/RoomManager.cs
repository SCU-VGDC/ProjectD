using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Wave {
    // an empty game object which contains all the enemies as children 
    public GameObject waveGameObject;

    // the delay before the wave starts
    public float waveDelayTime = 0.0f;
    // the delay between enemies
    public float enemyDelayTime = 0.0f;

    public bool waveOver 
    { 
        get 
        { 
            if (waveGameObject == null) return false;

            return waveGameObject.transform.childCount == 0; 
        } 
        set { value = waveOver; } 
    }

    public Wave(GameObject initialWaveGameObject, float initialWaveDelayTime, float initialEnemyDelayTime) 
    {
        waveGameObject = initialWaveGameObject;
        waveDelayTime = initialWaveDelayTime;
        enemyDelayTime = initialEnemyDelayTime;

        waveOver = false;

        // make all enemies not enabled
        foreach(Transform enemy in waveGameObject.transform) 
        {
            enemy.gameObject.SetActive(false);
        }
    }
}

public class RoomManager : MonoBehaviour
{
    public int ArenaId;

    private bool inBattle;  // if battle started
    private bool m_inBattle;
    public List<Wave> waves = new List<Wave>();
    private int waveNum;
    private IEnumerator coroutineWave;
    private IEnumerator coroutineSpawnWaves;

    private GameObject player;
    public GameObject doorCloser;
    private GameObject entrance;
    private GameObject exit;

    public float doorCloserDistance = 0.3f;

    public float waveDelayTime = 2.5f;
    public float enemySpawnDelayTime = 1.0f;

    public bool isCompleted;

    void Start()
    {
        player = GameManager.inst.player;

        foreach (Transform child in transform) // iterate over children
        {
            switch(child.gameObject.name) 
            {
                case "DoorCloser":
                    doorCloser = child.gameObject;
                    break;

                case "Entrance":
                    entrance = child.gameObject;
                    break;

                case "Exit":
                    exit = child.gameObject;
                    break;

                default:
                    if (child.name.Contains("Wave"))
                    {
                        waves.Add(new Wave(child.gameObject, waveDelayTime, enemySpawnDelayTime));
                    }

                    break;
            }
        }

        // default the gates open
        entrance.SetActive(false);
        exit.SetActive(false);
    }

    void FixedUpdate()
    {
        if(isCompleted)
        {
            return;
        }

        // check if player has entered the arena
        float playerDist = Vector2.Distance(player.transform.position, doorCloser.transform.position);
        if(playerDist < doorCloserDistance)
        {
            inBattle = true;
        }

        //check if battle started has been changed from last state
        if (inBattle != m_inBattle)
        {

            if (!inBattle) // battle has ended
            {
                entrance.SetActive(false); //open doors
                exit.SetActive(false);
            }


            if (inBattle) //set active all enemies and close doors
            {
                // boxed like a fish
                entrance.SetActive(true);
                exit.SetActive(true);

                coroutineSpawnWaves = spawnWaves();
                StartCoroutine(coroutineSpawnWaves);
            }

            m_inBattle = inBattle; //save the last value
        }
    }

    /// <summary>
    /// Spawns an individual wave
    /// </summary>
    /// <param name="wave"></param>
    /// <returns></returns>
    private IEnumerator spawnWave(Wave wave)
    {
        yield return new WaitForSeconds(wave.waveDelayTime);

        // spawn enemies
        foreach(Transform enemy in wave.waveGameObject.transform) 
        {
            enemy.gameObject.SetActive(true);

            yield return new WaitForSeconds(wave.enemyDelayTime);
        }
    }

    /// <summary>
    /// Spawns each wave sequentially
    /// </summary>
    /// <param name="wave"></param>
    /// <returns></returns>
    private IEnumerator spawnWaves()
    {
        for (waveNum = 0; waveNum < waves.Count; waveNum++) 
        {
            Wave currentWave = waves[waveNum];

            coroutineWave = spawnWave(currentWave);
            StartCoroutine(coroutineWave);

            // wait until wave has been completed (all enemies are dead)
            while (!currentWave.waveOver)
            {
                yield return null;
            }
        }

        // be free
        exit.SetActive(false);
        isCompleted = true;
    }


    void OnDrawGizmos()
    {
            if (doorCloser != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(doorCloser.transform.position, doorCloserDistance);
            }
   
    }
}
