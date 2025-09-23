using UnityEngine;


/// \file CameraFollow.cs
/// \brief Handles camera following car with set offset.
/// \ingroup Vehicle
public class CameraFollow : MonoBehaviour
{
    public Transform target;             // Car transform
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public float smoothSpeed = 5f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        transform.rotation = Quaternion.Euler(20f, 0f, 0f);
    }

    /// <summary>
    /// Positions camera behind the car with given offset
    /// </summary>
    /// <returns>
    /// None
    /// </returns>
    /// <remarks>
    /// Could add constrained slider for user choice (mmb?)
    /// </remarks>
    void LateUpdate()
    {
        if (!target) return;

        // Position the camera behind the car with offset
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);

        // Match the car's forward direction
        Quaternion targetRotation = Quaternion.LookRotation(target.forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }

}
