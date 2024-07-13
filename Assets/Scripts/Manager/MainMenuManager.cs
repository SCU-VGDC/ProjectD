using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsMenuUI;
    public GameObject mainMenuUI;

    public bool openSettingsFromInGame;
    void Start()
    {
        openSettingsFromInGame = false;

        // check if settings was opened from in game
        if (SceneManager.GetSceneByName("Redo Level 1-1").isLoaded)
        {
            openSettingsFromInGame = true;
            OpenSettingsMenu();
        }
    }

    public void OpenSettingsMenu()
    {
        settingsMenuUI.SetActive(true);
        mainMenuUI.SetActive(false);
    }

    public void CloseSettingsMenu()
    {
        if (!openSettingsFromInGame)
        {
            settingsMenuUI.SetActive(false);
            mainMenuUI.SetActive(true);
        }
        else
        {
            openSettingsFromInGame = false;

            SceneManager.UnloadSceneAsync("Title Screen");
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Redo Level 1-1");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
