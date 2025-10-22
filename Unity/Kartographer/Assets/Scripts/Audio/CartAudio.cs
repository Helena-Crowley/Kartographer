using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CartAudio : MonoBehaviour
{
    [SerializeField] private AudioSource engineA;
    [SerializeField] private AudioSource engineB;
    [SerializeField] private float minPitch = 1f;
    [SerializeField] private float maxPitch = 1.75f;
    [SerializeField] private AnimationCurve pitchCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float pitchSmooth = 5f;
    [SerializeField] private float stopPitchSpeed = 5f;    // how fast pitch goes to min before stopping
    [SerializeField] private float stopThreshold = 0.03f;  // normalized speed to start stopping

    private float currentPitch = 1f;
    private float halfLength;
    private bool stopping = false;

    private void Awake()
    {
        if (engineA != null && engineB != null && engineA.clip != null)
        {
            engineA.loop = false;
            engineB.loop = false;
            halfLength = engineA.clip.length / 2f;
        }
    }

    public void UpdatePitch(float normalizedSpeed)
    {
        if (engineA == null || engineB == null) return;

        float targetPitch;

        if (normalizedSpeed > stopThreshold)
        {
            // Car is moving normally
            stopping = false;
            targetPitch = Mathf.Lerp(minPitch, maxPitch, pitchCurve.Evaluate(normalizedSpeed));

            // Start engines if none are playing
            if (!engineA.isPlaying && !engineB.isPlaying)
            {
                engineA.Play();
                Invoke(nameof(PlayB), halfLength);
            }
        }
        else
        {
            // Car is slowing → pitch down before stopping
            stopping = true;
            targetPitch = minPitch;
        }

        // Smoothly interpolate pitch
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * (stopping ? stopPitchSpeed : pitchSmooth));
        engineA.pitch = currentPitch;
        engineB.pitch = currentPitch;

        // Stop AudioSources once pitch reaches minPitch
        if (stopping && currentPitch <= minPitch + 0.01f)
        {
            if (engineA.isPlaying) engineA.Stop();
            if (engineB.isPlaying) engineB.Stop();
            CancelInvoke(nameof(PlayB));
        }
    }

    private void PlayB()
    {
        if (engineB != null && !stopping)
        {
            engineB.Play();
            Invoke(nameof(PlayA), halfLength);
        }
    }

    private void PlayA()
    {
        if (engineA != null && !stopping)
        {
            engineA.Play();
            Invoke(nameof(PlayB), halfLength);
        }
    }
}
