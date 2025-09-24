using UnityEngine;
using UnityEngine.InputSystem;

public class CartInteraction : MonoBehaviour
{
    public Transform driverSeat;
    public Camera playerCam;
    public Camera cartCam;
    public GameObject player;
    public CharacterController characterController;
    public MonoBehaviour playerLook;
    public MonoBehaviour playerController;

    public GameObject interactPrompt;
    private bool nearCart = false;

    [SerializeField] private InputActionReference playerInteractAction;
    [SerializeField] private InputActionReference cartInteractAction;

    private bool inCart = false;

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
            interactPrompt.SetActive(true);
            nearCart = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactPrompt.SetActive(false);
            nearCart = false;
        }
    }

    void Update()
    {
        if (playerInteractAction.action.WasPressedThisFrame() || cartInteractAction.action.WasPressedThisFrame())
        {
            Debug.Log("Interact Pressed");
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

    void EnterCart()
    {
        playerController.enabled = false;
        playerLook.enabled = false;
        characterController.enabled = false;
        player.transform.SetParent(driverSeat);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;

        player.GetComponent<Animator>().SetBool("InCart", true);

        playerCam.enabled = false;
        cartCam.enabled = true;
        interactPrompt.SetActive(false);

        InputManager.Instance.EnterCart();
        inCart = true;
    }

    void ExitCart()
    {
        player.transform.SetParent(null);
        player.transform.position = driverSeat.position + driverSeat.right * 2f; // exit to the side
        player.GetComponent<Animator>().SetBool("InCart", false);

        cartCam.enabled = false;
        playerCam.enabled = true;
        //cartController.enabled = false;
        playerController.enabled = true;
        playerLook.enabled = true;
        characterController.enabled = true;

        InputManager.Instance.ExitCart();
        inCart = false;
    }
}
