using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class DoorInteract : MonoBehaviour
{
    public float rotationSpeed = 50f;

    private GameObject localPlayer;
    private InteractPrompt interactPrompt;
    private PlayerInputManager inputManager;
    private InputActionMap playerMap;
    private InputAction interactAction;

    private bool doorOpen;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the thing entering has a PlayerUI

        if (other.tag == "Player")
        {
            localPlayer = other.gameObject;

            interactPrompt = localPlayer.GetComponentInChildren<InteractPrompt>();
            inputManager = localPlayer.GetComponent<PlayerInputManager>();

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
        // if (localPlayer != null)
        // {
        //     playerMap = inputManager.controls.FindActionMap("Player", true);
        //     interactAction = playerMap.FindAction("Interact");

        //     if (interactAction.WasPerformedThisFrame() && !doorOpen)
        //     {
        //         transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        //     }
        // }
    }
}
