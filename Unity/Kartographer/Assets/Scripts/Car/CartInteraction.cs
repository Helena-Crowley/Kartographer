using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class CartInteraction : MonoBehaviour
{
    public Transform driverSeat;
    public Camera playerCam;
    public Camera cartCam;
    public CharacterController characterController;
    public MonoBehaviour playerLook;
    public MonoBehaviour playerController;

    private bool nearCart = false;

    [SerializeField] private InputActionReference playerInteractAction;
    [SerializeField] private InputActionReference cartInteractAction;

    private bool inCart = false;

    private GameObject localPlayer;
    private PlayerInputManager inputManager;
    private InteractPrompt interactPrompt;
    private Animator playerAnimator;

    void OnEnable()
    {
        playerInteractAction.action.Enable();
        cartInteractAction.action.Enable();
    }
    void OnDisable()
    {
        playerInteractAction.action.Disable();
        cartInteractAction.action.Disable();
    }

    void Start()
    {
        playerLook.enabled = true;
        playerController.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            localPlayer = other.gameObject;

            interactPrompt = localPlayer.GetComponent<InteractPrompt>();
            playerAnimator = localPlayer.GetComponent<Animator>();
            inputManager = localPlayer.GetComponent<PlayerInputManager>();

            interactPrompt?.ToggleInteractPrompt("E", "enter cart");

            nearCart = true;
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
        if (playerInteractAction.action.WasPressedThisFrame() || cartInteractAction.action.WasPressedThisFrame())
        {
            {
                if (!inCart && nearCart) EnterCart();
                else
                {
                    Debug.Log("Exiting Cart");
                    ExitCart();
                }
            }
        }
    }

    PlayerInputManager EnterCart()
    {
        playerController.enabled = false;
        playerLook.enabled = false;
        characterController.enabled = false;

        localPlayer.transform.SetParent(driverSeat);
        localPlayer.transform.localPosition = Vector3.zero;
        localPlayer.transform.localRotation = Quaternion.identity;

        playerCam.enabled = false;
        cartCam.enabled = true;

        playerAnimator?.SetBool("InCart", true);
        interactPrompt?.ToggleInteractPrompt();
        inputManager?.EnterCart();
        inCart = true;

        return inputManager;
    }

    void ExitCart()
    {
        localPlayer.transform.SetParent(null);
        localPlayer.transform.position = driverSeat.position + driverSeat.right * 2f; // exit to the side
        playerAnimator?.SetBool("InCart", false);

        cartCam.enabled = false;
        playerCam.enabled = true;
        //cartController.enabled = false;
        playerController.enabled = true;
        playerLook.enabled = true;
        characterController.enabled = true;

        inputManager?.ExitCart();
        inCart = false;
    }
}
