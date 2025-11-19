using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    [SerializeField] private Light directionalLight;
    [SerializeField] private float dayLengthSeconds = 60f;
    [SerializeField] private AnimationCurve lightIntensityCurve;
    [SerializeField] private AnimationCurve lightTempCurve;
    [SerializeField] private Vector3 initialLightRotation = new Vector3(16, -90, 0);
    [SerializeField] private Vector3 endLightRotation = new Vector3(16, 90, 0);
    [SerializeField] private float startTemperature = 40f;
    [SerializeField] private AudioClip warningVoice;

    private bool warned = false;

    //Temperature
    public float minTemperature = -29f;
    private float maxTemperature;
    public float currentTemperature;

    private VolumeProfile volumeProfile;

    //Exposure
    private Exposure exposure;
    private float expMin = 0.8f;
    private float expMax = 2.5f;

    //HDRI Sky
    private HDRISky hdriSky;
    private float rotationStart = 264f;
    private float rotationEnd = 0f;
    private float t = 0f;

    //Directional Light
    private int initialLightTemperature = 2800;
    private int endLightTemperature = 20000;
    private int initialLightIntensity = 500;
    private int endLightIntensity = 0;


    // Original values
    private float originalT;
    private float originalCurrentTemperature;
    private float originalHDRIRotation;
    private float originalExposure;
    private Quaternion originalLightRotation;
    private float originalLightIntensity;
    private float originalLightTemperature;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volumeProfile = globalVolume.profile;
        maxTemperature = startTemperature;
        currentTemperature = maxTemperature;

        originalT = t;
        originalCurrentTemperature = currentTemperature;

        if (volumeProfile.TryGet<HDRISky>(out HDRISky sky))
        {
            hdriSky = sky;
            originalHDRIRotation = hdriSky.rotation.value;
        }
        else
        {
            Debug.LogError("No HDRI Sky override found in the Volume.");
        }

        if (volumeProfile.TryGet<Exposure>(out Exposure exp))
        {
            exposure = exp;
            originalExposure = exposure.fixedExposure.value;
        }
        else
        {
            Debug.LogError("No exposure override found in the Volume.");
        }

        // Directional Light
        originalLightRotation = directionalLight.transform.rotation;
        originalLightIntensity = directionalLight.intensity;
        originalLightTemperature = directionalLight.colorTemperature;

        volumeProfile = globalVolume.profile;
        maxTemperature = startTemperature;
        currentTemperature = maxTemperature;

        if (volumeProfile.TryGet<HDRISky>(out sky))
        {
            hdriSky = sky;
        }
        else
        {
            Debug.LogError("No HDRI Sky override found in the Volume.");
        }

        if (volumeProfile.TryGet<Exposure>(out exp))
        {
            exposure = exp;
        }
        else
        {
            Debug.LogError("No exposure override found in the Volume.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
        UpdateGlobalVolume();
        UpdateDirectionalLight();
        UpdateTemperature();
    }

    void UpdateTimer()
    {
        t += Time.deltaTime / dayLengthSeconds;

        if (t > 1f) t = 1f; //stop at final spot
    }

    void UpdateGlobalVolume()
    {
        if (hdriSky == null) return;

        hdriSky.rotation.overrideState = true;
        hdriSky.rotation.value = Mathf.Lerp(rotationStart, rotationEnd, t);

        exposure.fixedExposure.value = Mathf.Lerp(expMin, expMax, t);
    }

    void UpdateDirectionalLight()
    {
        float sunAngle = Mathf.Lerp(initialLightRotation.y, endLightRotation.y, t);
        directionalLight.transform.rotation = Quaternion.Euler(initialLightRotation.x, sunAngle, 0);

        directionalLight.intensity = Mathf.Lerp(initialLightIntensity, endLightIntensity, lightIntensityCurve.Evaluate(t));
        directionalLight.colorTemperature = Mathf.Lerp(initialLightTemperature, endLightTemperature, lightTempCurve.Evaluate(t));
    }

    void UpdateTemperature()
    {
        currentTemperature = Mathf.Lerp(maxTemperature, minTemperature, t);
        if (currentTemperature <= -12 && !warned)
        {
            warned = true;
            SoundManager.Instance.PlaySound2D(warningVoice, "SFX", .15f);
        }
    }

    public void ResetDayNightCycle()
    {
        t = originalT;
        currentTemperature = originalCurrentTemperature;

        if (hdriSky != null)
            hdriSky.rotation.value = originalHDRIRotation;

        if (exposure != null)
            exposure.fixedExposure.value = originalExposure;

        if (directionalLight != null)
        {
            directionalLight.transform.rotation = originalLightRotation;
            directionalLight.intensity = originalLightIntensity;
            directionalLight.colorTemperature = originalLightTemperature;
        }

        warned = false; // reset warning trigger
    }

}
