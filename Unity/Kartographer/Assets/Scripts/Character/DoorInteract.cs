using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class DoorInteract : MonoBehaviour
{
    public float rotationSpeed = 50f;

    private GameObject localPlayer;
    private InteractPrompt interactPrompt;
    private InputAction interactAction;
    private InputActionMap playerMap;
    private PlayerInputManager inputManager;

    private bool doorOpen;
    private bool canOpen;
    public GameObject doorMesh;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    [HideInInspector]
    public bool slam = false;

    private void Start()
    {
        doorOpen = false;
        closedRotation = doorMesh.transform.rotation;
        openRotation = Quaternion.Euler(0, 90, 0) * closedRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            localPlayer = other.gameObject;

            interactPrompt = localPlayer.GetComponentInChildren<InteractPrompt>();
            inputManager = localPlayer.GetComponent<PlayerInputManager>();
            playerMap = inputManager.PlayerInput.actions.FindActionMap("Player", true);
            interactAction = playerMap.FindAction("Interact");

            interactPrompt?.ToggleInteractPrompt("E", "open door");
            canOpen = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == localPlayer)
        {
            localPlayer = null;

            interactPrompt?.ToggleInteractPrompt();

            canOpen = false;
        }
    }


    private void Update()
    {
        if (interactAction != null)
        {
            if (interactAction.WasPressedThisFrame() && canOpen)
            {
                doorOpen = !doorOpen;
            }
        }

        if (doorOpen)
            doorMesh.transform.rotation = Quaternion.RotateTowards(doorMesh.transform.rotation, openRotation, rotationSpeed * Time.deltaTime);
        else if (slam)
            doorMesh.transform.rotation = Quaternion.RotateTowards(doorMesh.transform.rotation, closedRotation, rotationSpeed * 2 * Time.deltaTime);
        else
            doorMesh.transform.rotation = Quaternion.RotateTowards(doorMesh.transform.rotation, closedRotation, rotationSpeed * Time.deltaTime);
    }
}
