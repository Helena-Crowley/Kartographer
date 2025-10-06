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
    public GameObject doorMesh;

    private Quaternion closedRotation;
    private Quaternion openRotation;

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
            playerMap = inputManager.controls.FindActionMap("Player", true);
            interactAction = playerMap.FindAction("Interact");

            interactPrompt?.ToggleInteractPrompt("E", "open door");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == localPlayer)
        {
            localPlayer = null;
            
            interactPrompt?.ToggleInteractPrompt();
        }
    }


    private void Update()
    {
        if (interactAction != null)
        {
            if (interactAction.WasPressedThisFrame())
            {
                doorOpen = !doorOpen;
            }
        }

        if (doorOpen)
            doorMesh.transform.rotation = Quaternion.RotateTowards(doorMesh.transform.rotation, openRotation, rotationSpeed * Time.deltaTime);
        else
            doorMesh.transform.rotation = Quaternion.RotateTowards(doorMesh.transform.rotation, closedRotation, rotationSpeed * Time.deltaTime);
    }
}
