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
    [SerializeField] private InputActionReference lookAction;

    private float cameraPitch = 0f;

    void OnEnable()
    {
        lookAction.action.Enable();
    }

    void OnDisable()
    {
        lookAction.action.Disable();
    }

    void Update()
    {
        
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;        

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
