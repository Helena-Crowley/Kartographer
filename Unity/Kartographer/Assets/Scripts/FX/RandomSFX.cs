using UnityEngine;
using System.Collections;

public class RandomSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clips;
    public float minDelay = 20f; // minimum seconds between sounds
    public float maxDelay = 110f; // maximum seconds between sounds

    private void Start()
    {
        StartCoroutine(PlayRandomSounds());
    }

    private IEnumerator PlayRandomSounds()
    {
        while (true)
        {
            // Wait a random amount of time
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            // Pick a random clip
            if (clips.Length > 0)
            {
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                SoundManager.Instance.PlaySound2D(clip, "Ambient", .01f);
            }
        }
    }
}
