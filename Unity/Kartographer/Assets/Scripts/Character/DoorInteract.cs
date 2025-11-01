using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class DoorInteract : NetworkBehaviour
{
    [Header("Door Settings")]
    public float rotationSpeed = 250f;
    public AudioClip doorOpenClip;
    public AudioClip doorCloseClip;
    public GameObject doorMesh;

    [HideInInspector]
    public bool slam = false;

    private GameObject localPlayer;
    private InteractPrompt interactPrompt;
    private InputAction interactAction;
    private InputActionMap playerMap;
    private PlayerInputManager inputManager;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool doorOpen = false;
    private bool canOpen = false;
    private bool hasPlayedSound = false;

    private void Start()
    {
        closedRotation = doorMesh.transform.rotation;
        openRotation = Quaternion.Euler(0, 90, 0) * closedRotation; // default
    }

    private void Update()
    {
        // Rotate door smoothly every frame
        if (doorOpen)
            doorMesh.transform.rotation = Quaternion.RotateTowards(doorMesh.transform.rotation, openRotation, rotationSpeed * Time.deltaTime);
        else if (slam)
            doorMesh.transform.rotation = Quaternion.RotateTowards(doorMesh.transform.rotation, closedRotation, rotationSpeed * 2 * Time.deltaTime);
        else
            doorMesh.transform.rotation = Quaternion.RotateTowards(doorMesh.transform.rotation, closedRotation, rotationSpeed * Time.deltaTime);

        // Check input for local player
        if (interactAction != null && interactAction.WasPressedThisFrame() && canOpen)
        {
            TryToggleDoor();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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

    private void TryToggleDoor()
    {
        // Determine direction to open based on player position
        Vector3 hingePosition = doorMesh.transform.position; // adjust if hinge is offset
        Vector3 toPlayer = localPlayer.transform.position - hingePosition;
        float dot = Vector3.Dot(doorMesh.transform.up, toPlayer);

        openRotation = Quaternion.Euler(0, (dot > 0 ? 90 : -90), 0) * closedRotation;

        hasPlayedSound = false; // reset sound flag

        if (IsServer)
        {
            ToggleDoor();
        }
        else
        {
            ToggleDoorServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleDoorServerRpc(ServerRpcParams rpcParams = default)
    {
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        doorOpen = !doorOpen;
        hasPlayedSound = false;

        // Broadcast sound to all clients
        PlayDoorSoundClientRpc(doorOpen);
    }

    [ClientRpc]
    private void PlayDoorSoundClientRpc(bool open)
    {
        if (open)
            SoundManager.Instance.PlaySound(doorOpenClip, transform.position, .3f, true);
        else
            SoundManager.Instance.PlaySound(doorCloseClip, transform.position, .4f, false, 1);
    }
}
