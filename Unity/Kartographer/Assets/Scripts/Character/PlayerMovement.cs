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
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference runAction;
    [SerializeField] private InputActionReference jumpAction;

    //public InputAction moveAction;   // Vector2
    //public InputAction runAction;    // Button
    //public InputAction jumpAction;   // Button

    public CharacterController controller;
    public PlayerStats playerStats;
    private Animator animator;
    private CameraBobbing bob;

    private float verticalVelocity = 0f;
    private Vector2 moveInput;

    [HideInInspector]
    public bool isRunning;
    public bool isWalking;

    [SerializeField]
    private Camera playerCamera; //used to get camera bobbing script

    private float turnSmoothVelocity;
    private bool exhausted = false;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        bob = playerCamera.GetComponent<CameraBobbing>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }

    void Update()
    {
        if (!moveAction.action.enabled) return;
        // --- Get Inputs ---
        moveInput = moveAction.action.ReadValue<Vector2>();
        bool runInput = runAction.action.ReadValue<float>() > 0.5f;

        if (playerStats.currentStamina <= 0)
        {
            exhausted = true;
        }

        // If stamina recovers above 25, leave exhausted state
        if (playerStats.currentStamina >= 30)
        {
            exhausted = false;
        }

        isRunning = runInput && !exhausted && playerStats.currentStamina > 0;
        bool jumpPressed = jumpAction.action.WasPressedThisFrame();




        // --- Movement ---
        Vector3 moveDir = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        float speed = isRunning ? runSpeed : walkSpeed;
        if (speed == walkSpeed) isWalking = true; else isWalking = false;

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        if (jumpPressed && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        if (isRunning && moveDir.magnitude > 0)
        {
            playerStats.DrainStamina();
        }
        else
        {
            playerStats.RegainStamina();
        }

        //Debug.Log($"isRunning: {isRunning}, stamina: {playerStats.currentStamina}, speed: {speed}");

        Vector3 velocity = moveDir * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);




        // --- Animator ---
        animator.SetFloat("Horizontal", isRunning ? moveInput.x * 2 : moveInput.x);
        animator.SetFloat("Vertical", isRunning ? moveInput.y * 2 : moveInput.y);
        animator.SetFloat("Speed", moveDir.magnitude * speed);

        if (bob != null)
        {
            bool isMoving = moveDir.magnitude > 0;
            if (isRunning && isMoving)
            {
                bob.isRunning = false;
                bob.isWalking = true;
            }
            else if (!isRunning && isMoving)
            {
                bob.isRunning = true;
                bob.isWalking = false;
            }
            else
            {
                bob.isRunning = false;
                bob.isWalking = false;
            }
        }
        else
        {
            Debug.LogWarning("bob.isRunning is null!");
        }
    }

    public void Interact()
    {
        animator.SetTrigger("Interact");
    }
}
