using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Components;

public class CartInteraction : NetworkBehaviour
{
    public Transform driverSeat;
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

    void Start()
    {
        playersInCart = GameManager.Instance.playersInCart;
        Debug.Log($"CartInteraction NetworkObjectId: {GetComponent<NetworkObject>().NetworkObjectId}, " +
          $"IsSpawned: {GetComponent<NetworkObject>().IsSpawned}");
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
            cartFollowLook = localPlayer.GetComponent<CameraManager>().cartCamera.GetComponent<CameraFollow>();
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

        if (playerInteractAction.action.WasPressedThisFrame())
        {
            if (nearCart) EnterCart();
        }

        else if (cartInteractAction.action.WasPressedThisFrame())
        {
            if (inCart) ExitCart();
        }

    }

    void EnterCart() => EnterCartServerRpc(localPlayer.GetComponent<NetworkObject>().OwnerClientId);


    void ExitCart() => ExitCartServerRpc(localPlayer.GetComponent<NetworkObject>().OwnerClientId);

    [ServerRpc(RequireOwnership = false)]
    void EnterCartServerRpc(ulong playerId)
    {
        var playerNetObj = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerId);
        var cartNetObj = GetComponent<NetworkObject>();

        if (playerNetObj == null) return;

        playerNetObj.TrySetParent(cartNetObj);
        playerNetObj.transform.SetPositionAndRotation(driverSeat.position, driverSeat.rotation);

        EnterCartClientRpc(playerId);

        playerNetObj.GetComponent<NetworkTransform>().enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    void ExitCartServerRpc(ulong playerId)
    {
        Debug.Log("ExitCartServerRpc called");
        var playerNetObj = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerId);
        if (playerNetObj == null) return;

        playerNetObj.transform.SetParent(null);

        ExitCartClientRpc(playerId);

        playerNetObj.GetComponent<NetworkTransform>().enabled = true;
    }


    [ClientRpc]
    void EnterCartClientRpc(ulong playerId)
    {
        GameObject player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerId).gameObject;

        player.transform.position = driverSeat.position;
        player.transform.rotation = driverSeat.rotation;

        if (player == null) return;

        bool isLocal = player.GetComponent<NetworkBehaviour>().IsOwner;

        Animator anim = player.GetComponent<Animator>();
        anim?.SetBool("InCart", true);

        if (isLocal)
        {
            var movement = player.GetComponent<PlayerMovement>();
            var look = player.GetComponent<MouseLook>();
            var characterController = player.GetComponent<CharacterController>();
            var cameraManager = player.GetComponent<CameraManager>();
            var inputManager = player.GetComponent<PlayerInputManager>();
            var prompt = player.GetComponent<InteractPrompt>();
            var cartFollow = cameraManager.cartCamera.GetComponent<CameraFollow>();

            movement.enabled = false;
            look.enabled = false;
            characterController.enabled = false;
            cartFollow.enabled = true;
            prompt?.ToggleInteractPrompt();
            inputManager?.EnableCartInputMap();
            inCart = true;

            cameraManager.HandleCartStateChanged(true);
        }

        AssignCarOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ClientRpc]
    void ExitCartClientRpc(ulong playerId)
    {
        Debug.Log("in Exit cart client");
        GameObject player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerId).gameObject;

        player.transform.position = driverSeat.position + driverSeat.right * 2f;
        player.transform.rotation = Quaternion.Euler(0f, driverSeat.rotation.eulerAngles.y + 90f, 0f);

        if (player == null) return;

        bool isLocal = player.GetComponent<NetworkBehaviour>().IsOwner;

        Animator anim = player.GetComponent<Animator>();
        anim?.SetBool("InCart", false);

        if (isLocal)
        {
            var movement = player.GetComponent<PlayerMovement>();
            var look = player.GetComponent<MouseLook>();
            var controller = player.GetComponent<CharacterController>();
            var cameraManager = player.GetComponent<CameraManager>();
            var inputManager = player.GetComponent<PlayerInputManager>();
            var cartFollow = cameraManager.cartCamera.GetComponent<CameraFollow>();

            movement.enabled = true;
            look.enabled = true;
            controller.enabled = true;
            cartFollow.enabled = false;
            inCart = false;
            inputManager?.DisableCartInputMap();
            cameraManager.HandleCartStateChanged(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AssignCarOwnershipServerRpc(ulong clientId)
    {
        NetworkObject carNetObj = GetComponent<NetworkObject>();
        carNetObj.ChangeOwnership(clientId);
    }



}