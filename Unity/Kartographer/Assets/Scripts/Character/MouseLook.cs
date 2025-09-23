using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;  // Assign your camera here

    [Header("Settings")]
    public float mouseSensitivity = 2f;
    public float upverticalLookLimit = 70f;
    public float downverticalLookLimit = -45f;

    [Header("Input")]
    public InputAction lookAction;  // Vector2 (mouse delta)

    private float cameraPitch = 0f;

    void OnEnable()
    {
        lookAction.Enable();
    }

    void OnDisable()
    {
        lookAction.Disable();
    }

    void Update()
    {
        
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        Debug.Log($"Mouse Input: {lookInput}, MouseX: {mouseX}, MouseY: {mouseY}");

        

        // --- Horizontal rotation: rotate player ---
        transform.Rotate(Vector3.up * mouseX);

        // --- Vertical rotation: rotate camera and head ---
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, downverticalLookLimit, upverticalLookLimit);

        

        // Rotate camera locally
        if (playerCamera != null)
            playerCamera.localEulerAngles = Vector3.right * cameraPitch;
    }
}
