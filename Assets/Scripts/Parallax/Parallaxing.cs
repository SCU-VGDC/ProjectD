using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public Transform[] backgrounds; //  Array list of all backgrounds and foregrounds
    private float[] parallaxScales; // Proportion of camera movement to backgrounds
    public float smoothing = 1;    // How smooth the parallax is going to be (>0)

    private Transform cam;          // Reference to main cam transform
    private Vector3 previousCamPos; // Position of camera in previous frame. 

    // Called before Start(), good for assigning variables
    void Awake() {
        cam  = Camera.main.transform;
    }

    // Start is called before the first frame update
    void Start() {
        previousCamPos  = cam.position;

        parallaxScales = new float[backgrounds.Length];

        for (int i = 0; i < backgrounds.Length; i++) {
            parallaxScales[i] = backgrounds[i].position.z*-1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++) {
            // Parallax is opposite of camera movement
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];

            // Set target x position (current pos plus parallax)
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;

            // Create a target pos which is background's current pos with it's target X
            Vector3 backgroundTargetPos = new Vector3 (backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            // Fade between current pos and target pos using lerp
            backgrounds[i].position = Vector3.Lerp (backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);

        }

        // set previousCamPos to cam's pos at end of frame
        previousCamPos = cam.position;
    }
}
