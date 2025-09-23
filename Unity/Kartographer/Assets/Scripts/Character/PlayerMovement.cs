using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float turnSmoothTime = 0.1f;


    [Header("Input")]
    public InputAction moveAction;   // Vector2
    public InputAction runAction;    // Button
    public InputAction jumpAction;   // Button

    private CharacterController controller;
    private Animator animator;

    private float verticalVelocity = 0f;
    private Vector2 moveInput;
    private bool isRunning;
    private float turnSmoothVelocity;

    void OnEnable()
    {
        moveAction.Enable();
        runAction.Enable();
        jumpAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        runAction.Disable();
        jumpAction.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Get Inputs ---
        moveInput = moveAction.ReadValue<Vector2>();
        isRunning = runAction.ReadValue<float>() > 0.5f;
        bool jumpPressed = jumpAction.WasPressedThisFrame();

        // --- Movement ---
        Vector3 moveDir = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        float speed = isRunning ? runSpeed : walkSpeed;

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        if (jumpPressed && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDir * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // --- Animator ---
        animator.SetFloat("Horizontal", isRunning ? moveInput.x * 2 : moveInput.x);
        animator.SetFloat("Vertical", isRunning ? moveInput.y * 2 : moveInput.y);
        animator.SetFloat("Speed", moveDir.magnitude * speed);
    }

    public void Interact()
    {
        animator.SetTrigger("Interact");
    }
}
