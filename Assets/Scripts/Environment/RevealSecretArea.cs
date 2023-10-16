using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealSecretArea : MonoBehaviour
{
    private bool isRevealed = false;
    [SerializeField] private AudioSource secretAreaSound;
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D other){
        if (!isRevealed && other.tag == "Player"){
            Debug.Log("Secret Area Found");
            isRevealed = true;
            secretAreaSound.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
