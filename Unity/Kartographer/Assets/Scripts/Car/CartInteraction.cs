using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class CartInteraction : NetworkBehaviour
{
    public Transform driverSeat;
    public Transform cameraTarget;
    private CharacterController characterController;
    private MouseLook firstPersonMouseLook;
    private PlayerMovement playerMovement;
    private PlayerStats playerStats;
    private CameraFollow cartFollowLook;

    private bool nearCart = false;

    [SerializeField] private InputActionReference playerInteractAction;
    [SerializeField] private InputActionReference cartInteractAction;

    private bool inCart = false;

    private GameObject localPlayer;
    private PlayerInputManager inputManager;
    private InteractPrompt interactPrompt;
    private Animator playerAnimator;

    private GameObject driver;

    private GameObject[] playersInCart;

    // void OnEnable()
    // {
    //     playerInteractAction.action.Enable();
    //     cartInteractAction.action.Enable();
    // }
    // void OnDisable()
    // {
    //     playerInteractAction.action.Disable();
    //     cartInteractAction.action.Disable();
    // }

    // void Awake()
    // {
        
    // }
    void Start()
    {
        playersInCart = GameManager.Instance.playersInCart;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            localPlayer = other.gameObject;

            interactPrompt = localPlayer.GetComponent<InteractPrompt>();

            interactPrompt?.ToggleInteractPrompt("E", "enter cart");

            nearCart = localPlayer.GetComponent<PlayerStats>().nearCart;
            nearCart = true;
            inCart = localPlayer.GetComponent<PlayerStats>().inCart;

            characterController = localPlayer.GetComponent<CharacterController>();
            playerMovement = localPlayer.GetComponent<PlayerMovement>();
            playerAnimator = localPlayer.GetComponent<Animator>();
            inputManager = localPlayer.GetComponent<PlayerInputManager>();
            firstPersonMouseLook = localPlayer.GetComponent<MouseLook>();
            cartFollowLook = localPlayer.GetComponent<CameraFollow>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == localPlayer)
        {
            interactPrompt?.ToggleInteractPrompt();

            playerAnimator = null;
            inputManager = null;
            interactPrompt = null;

            nearCart = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return; 

        if (playerInteractAction.action.WasPressedThisFrame())
        {
            if (nearCart) EnterCart();
        }

        else if (cartInteractAction.action.WasPressedThisFrame())
        {
            if (inCart) ExitCart();
        }

    }

    void EnterCart()
    {
        if (playersInCart[0] == null)
        {
            playersInCart[0] = localPlayer;
            driver = playersInCart[0];
            localPlayer.transform.SetParent(driverSeat); // make seat array for this
        }
        else if (playersInCart[1] == null)
        {
            playersInCart[1] = localPlayer;
        }
        else
        {
            Debug.Log("Cart is full");
            return; // cart is full
        }
        playerMovement.enabled = false;
        firstPersonMouseLook.enabled = false;
        characterController.enabled = false;

        // playercam do something
        // cartyfollow cam do something
        cartFollowLook.target = cameraTarget;
        cartFollowLook.enabled = true;

        localPlayer.transform.localPosition = Vector3.zero;
        localPlayer.transform.localRotation = Quaternion.identity;

        playerAnimator?.SetBool("InCart", true);
        interactPrompt?.ToggleInteractPrompt();
        inputManager?.EnableCartInputMap();
        inCart = true;
    }

    void ExitCart()
    {
        localPlayer.transform.SetParent(null);
        localPlayer.transform.position = driverSeat.position + driverSeat.right * 2f; // exit to the side
        localPlayer.transform.rotation = Quaternion.Euler(0f, driverSeat.rotation.eulerAngles.y + 90f, 0f);

        playerAnimator?.SetBool("InCart", false);

        playerMovement.enabled = true;
        firstPersonMouseLook.enabled = true;
        characterController.enabled = true;

        // playercam do something
        // cartyfollow cam do something
        cartFollowLook.enabled = false;
        inputManager?.DisableCartInputMap();
        inCart = false;

        if (localPlayer == driver)
        {
            playersInCart[0] = null;
            driver = null;
        }
        else if (localPlayer == playersInCart[1])
        {
            playersInCart[1] = null;
        }
    }
}