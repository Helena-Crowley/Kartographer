using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class MouseLook : NetworkBehaviour
{
    [Header("References")]
    public Transform playerCamera;

    [Header("Settings")]
    public float mouseSensitivity = 2f;
    public float upLimit = 70f;
    public float downLimit = -45f;

    private float cameraPitch = 0f;
    private PlayerInput playerInput;

    void Start () {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (!IsOwner) return; // only local player controls camera

        Vector2 look = playerInput.actions["Look"].ReadValue<Vector2>();

        float mouseX = look.x * mouseSensitivity;
        float mouseY = look.y * mouseSensitivity;

        // Horizontal rotation
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, downLimit, upLimit);

        if (playerCamera != null)
            playerCamera.localEulerAngles = Vector3.right * cameraPitch;
    }
}
