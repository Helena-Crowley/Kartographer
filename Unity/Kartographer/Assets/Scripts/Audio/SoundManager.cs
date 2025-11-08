using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (transform.parent != null) transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Plays a sound at a given position, optionally randomizing pitch, and destroys itself after playback.
    /// </summary>
    public void PlaySound(AudioClip clip, Vector3 position, string audioGroupName, float volume = 1f, bool randomPitchEnabled = false, float speed = 1)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = 1f; // 3D sound
        aSource.pitch = speed;

        aSource = AssignMixerGroup(audioGroupName, aSource);

        if (randomPitchEnabled)
            aSource.pitch = UnityEngine.Random.Range(0.7f, 1.2f) + speed - 1;

        aSource.Play();
        Destroy(tempGO, clip.length / Mathf.Abs(aSource.pitch)); // adjust for pitch speed
    }

    public void PlaySound2D(AudioClip clip, string audioGroupName, float volume = 1f, bool randomPitchEnabled = false, float speed = 1f)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio2D");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = 0f; // 2D sound
        aSource.pitch = speed;

        aSource = AssignMixerGroup(audioGroupName, aSource);

        if (randomPitchEnabled)
            aSource.pitch = UnityEngine.Random.Range(0.7f, 1.2f) + speed - 1;

        aSource.Play();
        Destroy(tempGO, clip.length / Mathf.Abs(aSource.pitch));
    }

    public AudioSource PlayLoopingSound(AudioClip clip, Vector3 position, string audioGroupName, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return null;

        GameObject tempGO = new GameObject("LoopingAudio");
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = 1f;
        aSource.pitch = pitch;
        aSource.loop = true;

        aSource = AssignMixerGroup(audioGroupName, aSource);

        aSource.Play();

        return aSource;
    }

    public void StopSound(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            Destroy(source.gameObject);
        }
    }

    private AudioSource AssignMixerGroup(string audioGroupName, AudioSource aSource)
    {
        if (audioMixer != null)
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(audioGroupName);
            if (groups.Length > 0)
            {
                aSource.outputAudioMixerGroup = groups[0];
            }
            else
            {
                groups = audioMixer.FindMatchingGroups("Master");
                aSource.outputAudioMixerGroup = groups[0];
                Debug.LogWarning($"Audio group '{audioGroupName}' not found in mixer. Using default output.");
            }

            return aSource;
        }

        return null;
    }
}

