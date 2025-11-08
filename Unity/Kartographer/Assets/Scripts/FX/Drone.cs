using UnityEngine;

public class Drone : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LineRenderer lineRenderer2;
    public float sweepAngle = 45f;
    public float sweepSpeed = 1f;
    public Transform drone;
    public float flySpeed = 10f;
    public AudioClip scanSFX;
    public AudioClip flyAwaySFX;

    [HideInInspector] public bool droneScanning = false;

    private float sweepTimer = .2f;
    private bool playedSound = false;
    private bool scanning = true;
    private float baseY;

    void Start()
    {
        baseY = drone.localPosition.y;
        ResetDrone();
    }

    void Update()
    {
        if (!droneScanning)
        {
            // Gentle up-and-down hover motion
            float hoverSpeed = 1.3f;      // oscillations per second
            float hoverHeight = 0.55f;  // amplitude of the oscillation
            float newY = baseY + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

            // Apply to local position
            Vector3 pos = drone.localPosition;
            pos.y = newY;
            drone.localPosition = pos;
            return;
        }
        ;
        if (scanning)
        {
            // Play scan sound once
            if (!playedSound)
            {
                SoundManager.Instance.PlaySound(scanSFX, transform.position, "SFX", 0.35f);
                playedSound = true;
            }

            lineRenderer.enabled = true;
            lineRenderer2.enabled = true;

            sweepTimer += Time.deltaTime;

            float angle = Mathf.Sin(sweepTimer * Mathf.PI * 2f * sweepSpeed) * sweepAngle;

            lineRenderer.transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
            lineRenderer2.transform.localRotation = Quaternion.Euler(-angle, 180f, 0f);

            // Stop scanning after one full sweep (-45 -> 45 -> -45)
            if (sweepTimer >= 1f / sweepSpeed) // one full cycle
            {
                scanning = false;
                GameManager.Instance.buildingsFound++;
                SoundManager.Instance.PlaySound2D(flyAwaySFX, "SFX",0.25f);
            }
        }
        else
        {
            // Fly away
            lineRenderer.enabled = false;
            lineRenderer2.enabled = false;
            drone.Translate(Vector3.forward * flySpeed * Time.deltaTime + Vector3.up * 0.02f);

            // Reset everything once drone is far enough (optional)
            // ResetDrone();
        }
    }

    public void ResetDrone()
    {
        droneScanning = false;
        scanning = true;
        sweepTimer = 0f;
        playedSound = false;
        lineRenderer.enabled = false;
        lineRenderer2.enabled = false;
        lineRenderer.transform.localRotation = Quaternion.identity;
        lineRenderer2.transform.localRotation = Quaternion.identity;
    }
}