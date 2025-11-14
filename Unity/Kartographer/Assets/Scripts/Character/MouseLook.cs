using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class MouseLook : NetworkBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    [SerializeField] private Transform playerHead;

    [Header("Settings")]
    public float mouseSensitivity = 2f;
    public float upLimit = 70f;
    public float downLimit = -45f;

    [SerializeField] private float headYawLimit = 45f;
    private float headYaw = 0f;
    private float cameraPitch = 0f;
    private PlayerInput playerInput;

    void Start()
    {
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

    void LateUpdate()
    {
        if (!IsOwner) return;

        Vector2 look = playerInput.actions["Look"].ReadValue<Vector2>();

        float mouseX = look.x * mouseSensitivity;
        float mouseY = look.y * mouseSensitivity;

        // Horizontal rotation
        //transform.Rotate(Vector3.up * mouseX);

        headYaw += mouseX;

        if (headYaw > headYawLimit)
        {
            float excess = headYaw - headYawLimit;
            transform.Rotate(Vector3.up * excess);
            headYaw = headYawLimit;
        }
        else if (headYaw < -headYawLimit)
        {
            float excess = headYaw + headYawLimit;
            transform.Rotate(Vector3.up * excess);
            headYaw = -headYawLimit;
        }


        // Vertical rotation
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, downLimit, upLimit);

        if (playerCamera != null)
            //playerCamera.localEulerAngles = Vector3.right * cameraPitch;
            playerCamera.localRotation = Quaternion.Euler(cameraPitch, headYaw, 0f);

        if (playerHead != null)
            //playerHead.localEulerAngles = Vector3.right * cameraPitch;
            playerHead.localRotation = Quaternion.Euler(cameraPitch, headYaw, 0f);
    }

}
