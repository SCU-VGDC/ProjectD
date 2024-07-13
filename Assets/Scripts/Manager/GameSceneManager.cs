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

    public GameObject blackImageObject;
    private UnityEngine.UI.Image blackImage;

    private bool fadeScreenCoroutineIsRunning;
    

    // singleton magic
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

        //DontDestroyOnLoad(inst.gameObject);
    }

    private void Start()
    {
        blackImage = blackImageObject.GetComponent<UnityEngine.UI.Image>();

        fadeScreenCoroutineIsRunning = false;
    }

    public IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public IEnumerator StartFadeScreen(bool transitionToScene, string nextScene)
    {
        if (fadeScreenCoroutineIsRunning) 
        {
            yield break;
        }

        fadeScreenCoroutineIsRunning = true;
        
        blackImageObject.SetActive(true);

        int iterations = 100;
        for (int i = 0; i < iterations; i++)
        {
            Color newColor = blackImage.color;
            newColor.a = ((float) i) / iterations;

            blackImage.color = newColor;

            yield return new WaitForSeconds(0.01f);
        }

        blackImageObject.SetActive(false);

        if (transitionToScene)
        {
            yield return StartCoroutine(LoadAsyncScene(nextScene));
        }

        fadeScreenCoroutineIsRunning = false;
    }
}
