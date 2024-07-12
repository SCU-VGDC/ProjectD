using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioRecord> AudioFiles = new();

    private readonly Dictionary<string, AudioRecord> AudioData = new();

    public string lastPlayedSound;
    public float lastPlayedSoundEndTime;

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
            // if same moving sound repeats and is still playing, exit
            // hard coded solution to repeated stepping sounds, fix in future if wanted
            if (name == lastPlayedSound && name == "Moving" && Time.time <= lastPlayedSoundEndTime) return;
            
            AudioRecord record = AudioData[name];
            AudioClip audioClip = record.clip;
            
            // randomizes variant if present
            if (record.Variants != null && record.Variants.Count > 0)
            {
                int rnd = Random.Range(0, record.Variants.Count);
                audioClip = record.Variants[rnd];
            }

            if (record.overlayPlay) 
            {
                audioSource.PlayOneShot(audioClip);
            } 
            else 
            {
                audioSource.clip = audioClip;
                audioSource.pitch = record.randomPitch ? Random.Range(0.9f, 1.1f) : 1;
                
                audioSource.Play();
            }

            // used to stop sounds cutting off other sounds. only used for Moving currently
            lastPlayedSound = name;
            lastPlayedSoundEndTime = Time.time + audioClip.length;
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

/// <summary>
/// If overlayPlay is true, will use audioSource.PlayOneShot(). If false, will use audioSource.Play()
/// </summary>
[System.Serializable]
public class AudioRecord
{
    public string name;
    public AudioClip clip;
    public List<AudioClip> Variants;
    public bool randomPitch;
    public bool overlayPlay;
}

