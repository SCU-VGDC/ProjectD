using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParralexEffect : MonoBehaviour
{
    private List<Transform> parLayers;
    private Vector3 origCamPos;

    void SetUpLayer()
    {
        parLayers = new List<Transform>();

        foreach (Transform child in transform)
        {
            parLayers.Add(child);
        }

        origCamPos = Camera.main.transform.position;

        foreach (Transform v in parLayers)
        {
            v.position = origCamPos;
        }
    }

    void Start()
    {
        SetUpLayer();
    }

    void MoveLayer(int LayerId)
    {
        Vector3 DistanceChange = Camera.main.transform.position - origCamPos;

        float moveFactor = (((float)(LayerId + 1)) / (parLayers.Count + 1));

        Vector3 AppliedPosition = DistanceChange * moveFactor;

        if (LayerId == 0)
        {
            AppliedPosition = DistanceChange;
        }

        parLayers[LayerId].position = AppliedPosition;
    }
    void LateUpdate()
    {
        for(int i = 0; i < parLayers.Count; i++)
        {
            MoveLayer(i);
        }
    }
}
