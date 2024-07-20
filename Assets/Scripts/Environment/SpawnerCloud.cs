using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpawnerCloud : MonoBehaviour
{
    private float totalAnimationLength = 1.0f;
    private float callCallbackTime = 1.0f;
    private Action destroyCallback;

    /// <summary>
    ///     Creates a Spawner Cloud object with customized position, animation timing, callback, and callback timing 
    /// </summary>
    /// <param name="instantiatePosition">The position the cloud is instantiated at</param>
    /// <param name="destroyCallback">A callback that runs at callCallbackTime seconds into the animation</param>
    /// <param name="callCallbackTime">How long (seconds) until the callback runs</param>
    /// <param name="formationLength">How long (seconds) the cloud formation part of the animation lasts</param>
    /// <param name="popLength">How long (seconds) the cloud pop part of the animation lasts</param>
    /// <returns></returns>
    public static SpawnerCloud Create(Vector3 instantiatePosition, Action destroyCallback, float callCallbackTime, float formationLength = 1f, float popLength = 1f)
    {
        // instantiate the object
        UnityEngine.Object prefab = Resources.Load("Prefabs/EnemyPrefabs/Spawner Cloud");
        GameObject newObject = Instantiate(prefab, instantiatePosition, Quaternion.identity) as GameObject;
        SpawnerCloud obj = newObject.GetComponent<SpawnerCloud>();

        Animator animator = obj.GetComponent<Animator>();

        // set animation speeds
        animator.SetFloat("Formation Speed", formationLength);
        animator.SetFloat("Pop Speed", popLength);

        // set object's variables
        obj.callCallbackTime = callCallbackTime;
        obj.totalAnimationLength = formationLength + popLength;
        obj.destroyCallback = destroyCallback;

        return obj;
    }

    void Start()
    {
        IEnumerator coroutineDestroy = destroyObj();
        StartCoroutine(coroutineDestroy);
    }


    private IEnumerator destroyObj()
    {
        yield return new WaitForSeconds(callCallbackTime);
        destroyCallback?.Invoke();

        // wait the remaining time of the animation
        yield return new WaitForSeconds(totalAnimationLength - callCallbackTime);
        Destroy(gameObject);
    }
}
