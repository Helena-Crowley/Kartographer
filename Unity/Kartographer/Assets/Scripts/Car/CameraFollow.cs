using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Offset")]
    public Transform target;
    public Vector3 followOffset = new Vector3(0f, 5f, -10f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Input")]
    public InputActionReference lookAroundAction;

    [Header("Settings")]
    public float orbitSpeed = 180f;
    public float followSmoothSpeed = 5f;
    public float transitionSmoothTime = 0.3f;
    public float verticalClampMin = -30f;
    public float verticalClampMax = 60f;

    private Vector3 currentOffset;
    private Vector3 velocity = Vector3.zero;

    private bool isOrbiting = false;

    // Spherical coordinates for orbit
    private float orbitYaw;
    private float orbitPitch;
    private float orbitDistance;

    private Transform cameraFirstPersonTransform;
    private Camera playerCam;
    private Vector3 tempTarget;
    //private float groundDistance; // distance from camera to the ground to prevent clipping
    //private float rayLength = 2.5f;
    private Vector3 tempLift;

    void Awake()
    {
        playerCam = GetComponentInChildren<Camera>();
    }
    void OnEnable()
    {
        lookAroundAction.action.Enable();
        cameraFirstPersonTransform = playerCam.transform;

        // Initialize camera behind car
        target = this.transform.root.transform;
        Debug.Log("CameraFollow target set to: " + target.name);

        currentOffset = followOffset;
        playerCam.transform.position = target.position + followOffset;
        playerCam.transform.LookAt(target);

        // Initialize orbit spherical coordinates from offset
        orbitDistance = followOffset.magnitude;
        orbitYaw = Mathf.Atan2(followOffset.x, followOffset.z) * Mathf.Rad2Deg;
        orbitPitch = Mathf.Asin(followOffset.y / orbitDistance) * Mathf.Rad2Deg;
    }

    void OnDisable()
    {
        lookAroundAction.action.Disable();
        playerCam.transform.position = cameraFirstPersonTransform.position;
        playerCam.transform.rotation = cameraFirstPersonTransform.rotation;
    }


    void Start()
    {
        // // Initialize camera behind car
        // currentOffset = followOffset;
        // playerCam.transform.position = target.position + followOffset;
        // playerCam.transform.LookAt(target);

        // // Initialize orbit spherical coordinates from offset
        // orbitDistance = followOffset.magnitude;
        // orbitYaw = Mathf.Atan2(followOffset.x, followOffset.z) * Mathf.Rad2Deg;
        // orbitPitch = Mathf.Asin(followOffset.y / orbitDistance) * Mathf.Rad2Deg;
    }

    void LateUpdate()
    {
        if (!target) return;

        bool looking = lookAroundAction.action.ReadValue<float>() > 0.5f;

        // Start orbit mode on right mouse press
        if (looking && !isOrbiting)
        {
            isOrbiting = true;

            // Convert current camera offset to spherical coordinates relative to target
            Vector3 offsetRel = playerCam.transform.position - target.position;
            orbitDistance = offsetRel.magnitude;
            orbitYaw = Mathf.Atan2(offsetRel.x, offsetRel.z) * Mathf.Rad2Deg;
            orbitPitch = Mathf.Asin(offsetRel.y / orbitDistance) * Mathf.Rad2Deg;
        }

        // Stop orbit mode on release
        if (!looking && isOrbiting)
        {
            isOrbiting = false;
        }

        Vector3 desiredOffset;

        if (isOrbiting)
        {
            // Update spherical coordinates based on mouse input
            float mouseX = Mouse.current.delta.x.ReadValue() * orbitSpeed * Time.deltaTime;
            float mouseY = Mouse.current.delta.y.ReadValue() * orbitSpeed * Time.deltaTime;

            orbitYaw += mouseX;
            orbitPitch -= mouseY;
            orbitPitch = Mathf.Clamp(orbitPitch, verticalClampMin, verticalClampMax);

            // Convert spherical coordinates back to Cartesian offset
            Quaternion rotation = Quaternion.Euler(orbitPitch, orbitYaw, 0f);
            desiredOffset = rotation * Vector3.forward * orbitDistance;
        }
        else
        {
            // Follow mode: always behind the car
            desiredOffset = target.TransformDirection(followOffset);
        }
        // Smoothly interpolate offset
        currentOffset = Vector3.SmoothDamp(currentOffset, desiredOffset, ref velocity, transitionSmoothTime);
        playerCam.transform.position = target.position + currentOffset;

        // Look at the car
        tempTarget = new Vector3(target.position.x, target.position.y + 2.5f, target.position.z);
        playerCam.transform.rotation = Quaternion.Slerp(playerCam.transform.rotation,
            Quaternion.LookRotation(tempTarget - playerCam.transform.position),
            followSmoothSpeed * Time.deltaTime);
    }
}