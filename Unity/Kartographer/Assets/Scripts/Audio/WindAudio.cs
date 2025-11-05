using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WindAudio : MonoBehaviour
{
    [SerializeField] private AudioSource windAudio;
    [SerializeField] private float minPitch = 1f;
    [SerializeField] private float maxPitch = 2.5f;
    [SerializeField] private float minVolume = 0f;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private AnimationCurve pitchCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve volumeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float stopThreshold = 0.05f;

    private float currentPitch;
    private float currentVolume;
    private void Awake()
    {
        if (windAudio != null)
        {
            if (windAudio.enabled == false) return;
            windAudio.loop = true; // wind is seamless
            windAudio.Play();
        }
    }

    public void UpdateWind(float normalizedSpeed)
    {
        if (windAudio == null) return;

        if (normalizedSpeed > stopThreshold)
        {
            // Target pitch and volume based on speed
            float targetPitch = Mathf.Lerp(minPitch, maxPitch, pitchCurve.Evaluate(normalizedSpeed));
            float targetVolume = Mathf.Lerp(minVolume, maxVolume, volumeCurve.Evaluate(normalizedSpeed));

            // Smoothly interpolate
            currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * smoothSpeed);
            currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // Fade out when slowing
            currentPitch = Mathf.Lerp(currentPitch, minPitch, Time.deltaTime * smoothSpeed);
            currentVolume = Mathf.Lerp(currentVolume, 0f, Time.deltaTime * smoothSpeed);
        }

        windAudio.pitch = currentPitch;
        windAudio.volume = currentVolume;
    }
}
