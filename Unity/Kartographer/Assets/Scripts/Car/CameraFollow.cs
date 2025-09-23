// using UnityEngine;


// /// \file CameraFollow.cs
// /// \brief Handles camera following car with set offset.
// /// \ingroup Vehicle
// public class CameraFollow : MonoBehaviour
// {
//     public Transform target;             // Car transform
//     public Vector3 offset = new Vector3(0f, 5f, -10f);
//     public float smoothSpeed = 5f;

//     private Vector3 velocity = Vector3.zero;

//     void Start()
//     {
//         transform.rotation = Quaternion.Euler(20f, 0f, 0f);
//     }

//     /// <summary>
//     /// Positions camera behind the car with given offset
//     /// </summary>
//     /// <returns>
//     /// None
//     /// </returns>
//     /// <remarks>
//     /// Could add constrained slider for user choice (mmb?)
//     /// </remarks>
//     void LateUpdate()
//     {
//         if (!target) return;

//         transform.LookAt(target);
//         // Position the camera behind the car with offset
//         Vector3 desiredPosition = target.position + target.TransformDirection(offset);
//         transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);

//         // Match the car's forward direction
//         Quaternion targetRotation = Quaternion.LookRotation(target.forward, Vector3.up);
//         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
//     }

// }

using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Offset")]
    public Transform target;
    public Vector3 followOffset = new Vector3(0f, 5f, -10f);

    [Header("Input")]
    public InputAction lookAroundAction;

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

    void OnEnable() => lookAroundAction.Enable();
    void OnDisable() => lookAroundAction.Disable();

    void Start()
    {
        // Initialize camera behind car
        currentOffset = followOffset;
        transform.position = target.position + followOffset;
        transform.LookAt(target);

        // Initialize orbit spherical coordinates from offset
        orbitDistance = followOffset.magnitude;
        orbitYaw = Mathf.Atan2(followOffset.x, followOffset.z) * Mathf.Rad2Deg;
        orbitPitch = Mathf.Asin(followOffset.y / orbitDistance) * Mathf.Rad2Deg;
    }

    void LateUpdate()
    {
        if (!target) return;

        bool looking = lookAroundAction.ReadValue<float>() > 0.5f;

        // Start orbit mode on right mouse press
        if (looking && !isOrbiting)
        {
            isOrbiting = true;

            // Convert current camera offset to spherical coordinates relative to target
            Vector3 offsetRel = transform.position - target.position;
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
        transform.position = target.position + currentOffset;

        // Look at the car
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(target.position - transform.position),
            followSmoothSpeed * Time.deltaTime);
    }
}
