using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioRecord> AudioFiles = new();

    private readonly Dictionary<string, AudioRecord> AudioData = new();

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        foreach (AudioRecord AudioFile in AudioFiles)
            AudioData.Add(AudioFile.name, AudioFile);
    }

    public void PlaySound(string name)
    {
        if (AudioData.ContainsKey(name))
        {
            AudioRecord record = AudioData[name];
            audioSource.clip = record.clip;

            // randomizes variant if present
            if (record.Variants != null && record.Variants.Count > 0)
            {
                int rnd = Random.Range(0, record.Variants.Count);
                audioSource.clip = record.Variants[rnd];
            }

            audioSource.pitch = record.randomPitch ? Random.Range(0.9f, 1.1f) : 1;
            audioSource.Play();
        }
        else
            Debug.LogWarning(gameObject + " requested to play a missing sound " + name);
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
    public bool randomPitch;
}

