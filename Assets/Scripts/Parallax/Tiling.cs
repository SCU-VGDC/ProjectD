using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour
{
    public int offsetX = 2; 

    public bool hasARightBuddy = false; // check if need to instantiate
    public bool hasALeftBuddy = false;

    public bool reverseScale = false; // used if object is not tilable

    private float spriteWidth = 0f; // width of texture
    private Camera cam;
    private Transform myTransform;

    void Awake () {
        cam = Camera.main;
        myTransform = transform;
    }

    // Start is called before the first frame update
    void Start() {
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = sRenderer.sprite.bounds.size.x * myTransform.localScale.x;
        
    }

    // Update is called once per frame
    void Update() {
        if (hasALeftBuddy == false || hasARightBuddy == false) { // does it need buddies?
            // calculates cameras extend (half of width) of what camera can see on screen
            float camHorizontalExtend = cam.orthographicSize * Screen.width/Screen.height;

            // calculate x pos where camera can see the edge of the sprite
            float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth/2) - camHorizontalExtend;
            float edgeVisiblePositionLeft = (myTransform.position.x - spriteWidth/2) + camHorizontalExtend;

            // checks if we see the edge of the element then makes a new buddy if you can
            if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && hasARightBuddy == false) {
                MakeNewBuddy (1);
                hasARightBuddy = true;
            } else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && hasALeftBuddy == false) {
                MakeNewBuddy (-1);
                hasALeftBuddy = true;
            }
        }
        
    }

    // creates buddy on side required
    void MakeNewBuddy (int rightOrLeft) {
        // calculates new pos for new buddy
        Vector3 newPosition = new Vector3 (myTransform.position.x + spriteWidth * rightOrLeft, myTransform.position.y, myTransform.position.z);
        // instantiating new buddy and storing it in variable
        Transform newBuddy = Instantiate (myTransform, newPosition, myTransform.rotation) as Transform;

        // if not tilable then we reverse the x size of our objecrt to get rid of ugly seams
        if (reverseScale == true) {
            newBuddy.localScale = new Vector3 (newBuddy.localScale.x*-1, newBuddy.localScale.y, newBuddy.localScale.z);
        }

        newBuddy.parent = myTransform;
        newBuddy.localScale = new Vector3(1,1,1);

        if (rightOrLeft > 0) {
            newBuddy.GetComponent<Tiling>().hasALeftBuddy = true;
        }
        else {
            newBuddy.GetComponent<Tiling>().hasARightBuddy = true;
        }
    }
}
