using Player_Movement_Namespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public bool battleStarted;
    public List<Enemy_Health> enemies;
    public int amountOfEnemies;

    private GameObject entrance;
    private GameObject exit;
    private bool m_battleStarted;
   

    void Start()
    {
        

        foreach (Transform enem in transform)
        {
            if (enem.transform.childCount != 0)
            {
                foreach (Transform undprefab in enem.transform)
                {
                    if (undprefab.GetComponent<Enemy_Health>())
                    {
                        enemies.Add(undprefab.GetComponent<Enemy_Health>());
                        undprefab.gameObject.SetActive(false);
                        amountOfEnemies++;
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

    public void Startbattle()
    {
        if(amountOfEnemies > 0)
        {
            battleStarted = true;
        }
    }

    
    void FixedUpdate()
    {
        if(amountOfEnemies == 0)
        {
            battleStarted = false;
        }

        if (battleStarted != m_battleStarted)
        {

            if (battleStarted == false)
            {
                entrance.SetActive(false);
                exit.SetActive(false);
            }


            if (battleStarted == true)
            {
                foreach(Enemy_Health i in enemies)
                {
                    i.gameObject.SetActive(true);
                }
                entrance.SetActive(true);
                exit.SetActive(true);
            }

            m_battleStarted = battleStarted;
        }
    }
}
