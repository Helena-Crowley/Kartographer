using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class CartAudio : MonoBehaviour
{
    [Header("Cart SFX")]
    [SerializeField] private AudioClip windSound;
    [SerializeField] private AudioClip tickSound;
    [SerializeField] private AudioClip honkSound;
    [SerializeField] private AudioClip[] suspensionSqueak;


    [SerializeField] private AudioSource engineA;
    [SerializeField] private AudioSource engineB;
    [SerializeField] private AnimationCurve pitchCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float pitchSmooth = 5f;
    [SerializeField] private float stopPitchSpeed = 5f;
    [SerializeField] private float stopThreshold = 0.03f;
    [SerializeField] private InputActionReference honkAction;

    private AudioSource golfCartAudioSource;

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

    private void Start()
    {
        golfCartAudioSource = gameObject.AddComponent<AudioSource>();
        golfCartAudioSource.spatialBlend = 1f;
        golfCartAudioSource.volume = .1f;
    }

    void Update()
    {
        if (honkAction.action.WasPressedThisFrame())
        {
            SoundManager.Instance.PlaySound(honkSound, transform.position, "SFX", .2f);
        }
    }

    public void PlaySpeedDependentSound(float normalizedSpeed)
    {
        UpdatePitch(normalizedSpeed);
    }

    public void PlayTickSound()
    {
            golfCartAudioSource.PlayOneShot(tickSound);
    }

    private void UpdatePitch(float normalizedSpeed, float minPitch = 2.5f, float maxPitch = 3.5f)
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
