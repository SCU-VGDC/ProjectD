using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class ParralexEffect : MonoBehaviour
{
    private Vector3 lastCameraPos;
    private int numLayers;
    private Transform[] parLayers;
    private float[] textureUnitSizesX;
    [SerializeField] private Vector2[] paralaxEffectMults;

    void Start()
    {
        //set last camera pos
        lastCameraPos = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.transform.position;

        //initialize numLayers
        numLayers = transform.childCount;

        //allocate space for vars
        parLayers = new Transform[numLayers];
        textureUnitSizesX = new float[numLayers];

        for(int i = 0; i < numLayers; i++)
        {
            parLayers[i] = transform.GetChild(i);

            Sprite sprite = parLayers[i].GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            textureUnitSizesX[i] = texture.width / sprite.pixelsPerUnit;
        }
    }

    void LateUpdate()
    {
        Vector3 deltaCamMovement = Camera.main.transform.position - lastCameraPos;

        //for each paralax layer
        for(int i = 0; i < numLayers; i++)
        {
            parLayers[i].position += new Vector3(deltaCamMovement.x * paralaxEffectMults[i].x, deltaCamMovement.y * paralaxEffectMults[i].y, 0f);

            if(Camera.main.transform.position.x - parLayers[i].position.x >= textureUnitSizesX[i])
            {
                float offsetPosX = (Camera.main.transform.position.x - transform.position.x) % textureUnitSizesX[i];
                transform.position = new Vector3(Camera.main.transform.position.x, parLayers[i].position.x);
            }
        }

        //get last camera pos again
        lastCameraPos = Camera.main.transform.position;
    }
}
