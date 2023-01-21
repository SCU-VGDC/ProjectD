using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Buttons : MonoBehaviour
{
    public void Play_Game()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit_Game()
    {
        Debug.Log("Game quit to desktop");
        Application.Quit();
    }
}
