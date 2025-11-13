using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public AudioMixer audioMixer;

    [Header("Dynamic music")]
    private AudioSource musicSource;
    private Coroutine musicFadeCoroutine;


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

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.spatialBlend = 0f; // 2D music
        musicSource.playOnAwake = false;

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

    /// <summary>
    /// Plays sound with range parameters
    /// </summary>
    /// <param name="rangeMin"></param>
    /// <param name="rangeMax"></param>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="audioGroupName"></param>
    /// <param name="volume"></param>
    /// <param name="randomPitchEnabled"></param>
    /// <param name="speed"></param>
    public void PlaySound(float rangeMin, float rangeMax, AudioClip clip, Vector3 position, string audioGroupName, float volume = 1f, bool randomPitchEnabled = false, float speed = 1)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = 1f;
        aSource.pitch = speed;
        aSource.minDistance = rangeMin;
        aSource.maxDistance = rangeMax;

        aSource = AssignMixerGroup(audioGroupName, aSource);

        if (randomPitchEnabled)
            aSource.pitch = UnityEngine.Random.Range(0.7f, 1.2f) + speed - 1;

        aSource.Play();
        Destroy(tempGO, clip.length / Mathf.Abs(aSource.pitch));
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

    public void PlayMusic(AudioClip clip, string audioGroupName, float volume = 1f, float fadeDuration = 1f)
    {
        if (clip == null) return;

        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        AssignMixerGroup(audioGroupName, musicSource);
        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();

        musicFadeCoroutine = StartCoroutine(FadeAudio(musicSource, volume, fadeDuration));
    }

    public void StopMusic(float fadeDuration = 1f)
    {
        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        musicFadeCoroutine = StartCoroutine(FadeOutAndStop(musicSource, fadeDuration));
    }

    private IEnumerator FadeAudio(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    private IEnumerator FadeOutAndStop(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }


}

