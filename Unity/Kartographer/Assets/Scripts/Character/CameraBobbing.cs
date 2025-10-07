using UnityEngine;

[RequireComponent(typeof(Transform))]
public class CameraBobbing : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobFrequency = 10f;   // how fast it bobs
    public float bobHeight = 0.05f;    // how high it moves
    public float bobSmooth = 5f;       // smooth factor

    [Header("Sway Settings")]
    public float swayAngle = 1f;       // maximum roll angle in degrees
    public float swayFrequency = 5f;   // speed of sway


    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isWalking = false; // set this from PlayerMovement
    private Vector3 startLocalPos;
    private float bobTimer = 0f;
    private Quaternion startLocalRot;

    void Start()
    {
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;
    }

    void Update()
    {
        Vector3 targetPos = startLocalPos;

        if (isRunning || isWalking)
        {
            // Adjust frequency and height depending on running vs walking
            float frequency = isRunning ? bobFrequency / 1.25f : bobFrequency;
            float height = isRunning ? bobHeight / 3f : bobHeight;

            bobTimer += Time.deltaTime * frequency;

            targetPos.y += Mathf.Sin(bobTimer) * height;
            targetPos.x += Mathf.Cos(bobTimer * 0.5f) * height * 0.5f;
        }
        else
        {
            // Reset timer so it starts smoothly when you run again
            bobTimer = 0f;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * bobSmooth);

        // Calculate sway based on bobTimer
        Quaternion baseRot = transform.localRotation; // current rotation from MouseLook

        float swayMultiplier = isRunning ? 2f : 1f;
        float currentSwayFreq = isRunning ? swayFrequency * 1.5f : swayFrequency;
        float sway = Mathf.Sin(bobTimer * currentSwayFreq) * swayAngle * swayMultiplier;

        Quaternion swayRot = Quaternion.Euler(0f, 0f, sway);

        // Apply sway on top of current rotation
        transform.localRotation = baseRot * swayRot;





    }
}
