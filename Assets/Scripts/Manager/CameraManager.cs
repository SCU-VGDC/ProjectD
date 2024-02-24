using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager inst;

    public Collider2D defaultBoundary;
    private CinemachineConfiner2D cinemachineConfiner2D;

    void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        Debug.Log("Ran");

        resetCameraBoundary();
    }

    /// <summary>
    ///  NOTE: The polygon has to be larger than the VM camera size for it to work correctly
    /// </summary>
    /// <param name="boundaryToChangeTo"></param>
    public void setCameraBoundary(Collider2D boundaryToChangeTo) 
    {
        GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = boundaryToChangeTo;
    }

    public void resetCameraBoundary()
    {
        setCameraBoundary(defaultBoundary);
    }
}
