// using UnityEngine;
// using UnityEngine.InputSystem;
// using Unity.Netcode;

// public class MouseLook : NetworkBehaviour
// {
//     [Header("References")]
//     public Transform playerCamera;  // Assign your camera here

//     [Header("Settings")]
//     public float mouseSensitivity = 2f;
//     public float upverticalLookLimit = 70f;
//     public float downverticalLookLimit = -45f;

//     [Header("Input")]
//     [SerializeField] private InputActionReference lookAction;

//     private float cameraPitch = 0f;

//     void OnEnable()
//     {
//         if (IsOwner) // only local player reads input
//             lookAction.action.Enable();
//     }

//     void OnDisable()
//     {
//         if (IsOwner)
//             lookAction.action.Disable();
//     }

//     void Update()
//     {
//         if (!IsOwner) return; // remote players do nothing

//         Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
//         float mouseX = lookInput.x * mouseSensitivity;
//         float mouseY = lookInput.y * mouseSensitivity;

//         // --- Horizontal rotation: rotate player ---
//         transform.Rotate(Vector3.up * mouseX);

//         // --- Vertical rotation: rotate camera ---
//         cameraPitch -= mouseY;
//         cameraPitch = Mathf.Clamp(cameraPitch, downverticalLookLimit, upverticalLookLimit);

//         if (playerCamera != null)
//             playerCamera.localEulerAngles = Vector3.right * cameraPitch;
//     }
// }
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
