using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Menu_Buttons : MonoBehaviour
{
    public static bool is_paused = false;

    public GameObject pause_menu_UI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(is_paused == true)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pause_menu_UI.SetActive(false);
        Time.timeScale = 1f;
        is_paused = false;
    }

    void Pause()
    {
        pause_menu_UI.SetActive(true);
        Time.timeScale = 0f;
        is_paused = true;
    }

    public void Settings()
    {
        Debug.Log("Settings 'Opened' (not really)");
    }

    public void Quit_to_Main()
    {
        is_paused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Quit_Game()
    {
        Debug.Log("Game quit to desktop");
        Application.Quit();
    }
}
