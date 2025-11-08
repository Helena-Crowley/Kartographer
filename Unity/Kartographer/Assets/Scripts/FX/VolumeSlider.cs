using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public AudioMixer mixer;     // Assign your AudioMixer in the Inspector
    public Slider volumeSlider;  // Assign your UI Slider
    public string volumeParam = "Master"; // The exposed mixer parameter name

    private void Start()
    {
        // Initialize slider with current volume
        if (mixer.GetFloat(volumeParam, out float currentVolume))
        {
            volumeSlider.value = Mathf.Pow(10f, currentVolume / 20f);
        }

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float value)
    {
        // Avoid log(0) error by clamping value
        value = Mathf.Clamp(value, 0.0001f, 1f);

        // Convert linear [0–1] to dB [-80, 0]
        float dB = Mathf.Log10(value) * 20f;
        mixer.SetFloat(volumeParam, dB);
    }
}
