using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioRecord> AudioFiles = new List<AudioRecord>();

    private Dictionary<string, AudioRecord> AudioData = new Dictionary<string, AudioRecord>();

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        foreach (AudioRecord AudioFile in AudioFiles)
        {
            AudioData.Add(AudioFile.name, AudioFile);
        }
    }

    public void PlaySound(string name)
    {
        if (AudioData.ContainsKey(name))
        {
            audioSource.clip = AudioData[name].clip;

            if (AudioData[name].Variants != null)
            {
                if (AudioData[name].Variants.Count > 0) //randomizes variant if present
                {
                    int rnd = Random.Range(0, AudioData[name].Variants.Count);
                    audioSource.clip = AudioData[name].Variants[rnd];
                }
            }

            audioSource.pitch = Random.Range(0.9f, 1.1f); //randomizes pitch for fun

            audioSource.Play();
        }
        else
        {
            Debug.LogWarning(gameObject + " requested to play a missing sound " + name);
        }
    }

    public void ChangeSoundInDict(string oldSound, string newSound)
    {
        if (AudioData.ContainsKey(newSound)) //check that we have a new sound
        {
            if (AudioData.ContainsKey(oldSound)) //crfeate an old sound if needed
            {
                AudioData[oldSound].clip = AudioData[newSound].clip;
            }
            else
            {
                AudioRecord ar = new AudioRecord();
                ar.name = oldSound;
                ar.clip = AudioData[newSound].clip;
                AudioData.Add(oldSound, ar);
            }
        }
        else
        {
            Debug.LogWarning(gameObject + " requested to change into missing sound " + newSound);
        }
    }
}

[System.Serializable]
public class AudioRecord
{
    public string name;
    public AudioClip clip;
    public List<AudioClip> Variants;
}

