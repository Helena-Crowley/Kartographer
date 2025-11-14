
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : NetworkBehaviour
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

    public CharacterController controller;
    public PlayerObj playerStats;
    private Animator animator;
    private CameraBobbing bob;

    private float verticalVelocity = 0f;
    private Vector2 moveInput;

    [HideInInspector] public bool isRunning;
    public bool isWalking;

    [SerializeField] private Camera playerCamera;

    private bool exhausted = false;
    private PlayerInput playerInput;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        if (playerCamera != null)
            bob = playerCamera.GetComponent<CameraBobbing>();

    }

    void Update()
    {
        if (!IsOwner) return; // <<< KEY FIX

        // --- Get Inputs ---
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        bool runInput = runAction.action.ReadValue<float>() > 0.5f;

        if (playerStats.currentStamina <= 0)
            exhausted = true;
        if (playerStats.currentStamina >= 30)
            exhausted = false;

        isRunning = runInput && !exhausted && playerStats.currentStamina > 0;
        bool jumpPressed = jumpAction.action.WasPressedThisFrame();

        // --- Movement ---
        Vector3 moveDir = (playerCamera.transform.forward * moveInput.y + playerCamera.transform.right * moveInput.x).normalized;
        float speed = isRunning ? runSpeed : walkSpeed;
        isWalking = (speed == walkSpeed);

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
        if (jumpPressed && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        if (isRunning && moveDir.magnitude > 0)
            playerStats.DrainStamina();
        else
            playerStats.RegainStamina();

        Vector3 velocity = moveDir * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // --- Animator ---
        if (IsOwner)
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
            animator.SetFloat("Speed", moveDir.magnitude * speed);


            // Send to server for syncing
            SendAnimParametersServerRpc(moveInput.x, moveInput.y, moveDir.magnitude * speed, isRunning);
        }


        // --- Camera bob ---
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
    }

    public void Interact()
    {
        if (!IsOwner) return; // don’t allow remote clients to trigger
        animator.SetTrigger("Interact");
    }

    [ServerRpc]
    private void SendAnimParametersServerRpc(float horizontal, float vertical, float speed, bool isRunning)
    {
        // Broadcast to all clients except the owner
        UpdateAnimParametersClientRpc(horizontal, vertical, speed, isRunning);
    }

    [ClientRpc]
    private void UpdateAnimParametersClientRpc(float horizontal, float vertical, float speed, bool running)
    {
        if (IsOwner) return; // owner already has correct Animator

        // Apply movement
        animator.SetFloat("Horizontal", running ? horizontal * 2f : horizontal);
        animator.SetFloat("Vertical", running ? vertical * 2f : vertical);
        animator.SetFloat("Speed", speed);
    }


}
