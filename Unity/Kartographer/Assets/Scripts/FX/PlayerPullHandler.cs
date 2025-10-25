using UnityEngine;
using Unity.Netcode;

public class PlayerPullHandler : NetworkBehaviour
{
    private bool isPulling = false;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float timer = 0f;
    private float duration = 15f;
    private float lockDuration = 5f;
    private DoorInteract door;
    private AudioSource audioSource;
    private GameObject fxGameObject;
    private VideoSwitcher videoSwitcher;

    public void StartPull(Vector3 pullTarget, float pullDuration, DoorInteract doorToClose, AudioSource tvSound, GameObject fxGO, VideoSwitcher videoSwitchScript)
    {
        if (!IsOwner) return; // only move locally for the owning client

        startPos = transform.position;
        targetPos = pullTarget;
        duration = pullDuration;
        door = doorToClose;
        audioSource = tvSound;
        fxGameObject = fxGO;
        timer = 0f;
        isPulling = true;
        videoSwitcher = videoSwitchScript;

        var move = GetComponent<PlayerMovement>();
        if (move) move.enabled = false;
    }

    void Update()
    {
        if (!isPulling) return;

        timer += Time.deltaTime;
        float t = Mathf.SmoothStep(0f, 1f, timer / duration);
        transform.position = Vector3.Lerp(startPos, targetPos, t);
        audioSource.volume = Mathf.Clamp(Mathf.Pow((t), 2f), 0f, 0.06f);
        audioSource.pitch = 0.05f;

        if (t >= 1f)
        {
            isPulling = false;
            if (door != null)
                door.slam = true;

            Invoke(nameof(ReenableMovement), lockDuration);
        }
    }

    void ReenableMovement()
    {
        var move = GetComponent<PlayerMovement>();
        audioSource.volume = 0;
        if (move) move.enabled = true;
        videoSwitcher.SwitchToClip2();

        Destroy(fxGameObject, 3f);
    }
}
