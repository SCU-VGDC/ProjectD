using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    /// <summary>
    /// This variable acts as a way to access any information contained in the SceneManager.
    /// It will always be equal to the SceneManager in the active scene, if it exists.
    /// </summary>
    public static GameSceneManager inst;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }

    void Transition(Scene scene)
    {
        //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync()
    }
}
