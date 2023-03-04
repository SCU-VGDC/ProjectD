using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Limit : MonoBehaviour
{
    public int target_frame_rate;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = target_frame_rate;
    }
}
