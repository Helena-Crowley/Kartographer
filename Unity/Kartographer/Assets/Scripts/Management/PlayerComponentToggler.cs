using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerComponentToggler : NetworkBehaviour
{
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private CharacterController playerController;
        [SerializeField] private MouseLook mouseLook;
        [SerializeField] private Inventory inventory;
        [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject cartCamera;
    [SerializeField] private PlayerInput playerInput;

    private void Awake()
    {
        playerMovement.enabled = false;
        playerController.enabled = false;
        mouseLook.enabled = false;
        inventory.enabled = false;
        playerCamera.SetActive(false);
        cartCamera.SetActive(false);
        playerInput.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            playerInput.enabled = true;
            playerController.enabled = true;
            mouseLook.enabled = true;
            inventory.enabled = true;
            playerCamera.SetActive(true);
            cartCamera.SetActive(true);
            playerMovement.enabled = true;
        }

    }
}
