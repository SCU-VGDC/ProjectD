using Player_Movement_Namespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public int id; //room id to save
    public bool battleStarted;  //if battle started
    public List<Enemy_Health> enemies; //list of enemies elements

    private GameObject entrance;
    private GameObject exit;
    private bool m_battleStarted;
   

    void Start()
    {
        foreach (Transform enem in transform) //finds enemies and entrance and exit
        {
            // if enemy's children does contain enemy health script
            if (enem.transform.childCount != 0)
            {
                // check parent for enemy health script
                if (enem.GetComponent<Enemy_Health>())
                {
                    addToEnemyList(enem);
                }

                // check children for enemy health script
                foreach (Transform undprefab in enem.transform)
                {
                    if (undprefab.GetComponent<Enemy_Health>())
                    {
                        addToEnemyList(undprefab);
                    }
                }
            }

            if (enem.name == "Entrance")
            {
                entrance = enem.gameObject;
            }
            if (enem.name == "Exit")
            {
                exit = enem.gameObject;
            }
        }

        entrance.SetActive(false);
        exit.SetActive(false);
    }

    void addToEnemyList(Transform enem)
    {
        enemies.Add(enem.GetComponent<Enemy_Health>());
        enem.gameObject.SetActive(false);
    }

    public void Startbattle() //startbattle trigger
    {
        if(enemies.Count > 0)
        {
            battleStarted = true;
        }
    }

    
    void FixedUpdate()
    {
        if(enemies.Count == 0)
        {
            battleStarted = false; //reset if no enemies
        }

        if (battleStarted != m_battleStarted) //check if battle started have been changed
        {

            if (battleStarted == false)
            {
                entrance.SetActive(false); //open doors
                exit.SetActive(false);
            }


            if (battleStarted == true) //set active all enemies and close doors
            {
                foreach(Enemy_Health i in enemies)
                {
                    i.gameObject.SetActive(true);
                }
                entrance.SetActive(true);
                exit.SetActive(true);
            }

            m_battleStarted = battleStarted; //save the last value
        }
    }
}
