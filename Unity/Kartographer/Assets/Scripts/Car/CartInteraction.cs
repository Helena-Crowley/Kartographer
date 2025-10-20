using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Components;

public class CartInteraction : NetworkBehaviour
{
    //public Transform driverSeat;

    private CharacterController characterController;
    private MouseLook firstPersonMouseLook;
    private PlayerMovement playerMovement;
    private PlayerStats playerStats;
    private CameraFollow cartFollowLook;
    private GameObject localPlayer;
    private PlayerInputManager inputManager;
    private InteractPrompt interactPrompt;
    private Animator playerAnimator;

    private PlayerInput playerInput;

    private int playersInCart = 0;

    public Transform[] seatPositions; //0 = driver seat

    void Start()
    {
        Debug.Log($"CartInteraction NetworkObjectId: {GetComponent<NetworkObject>().NetworkObjectId}, " +
          $"IsSpawned: {GetComponent<NetworkObject>().IsSpawned}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playersInCart == 3) return;

        var netObj = other.GetComponent<NetworkObject>();
        if (netObj == null) return;

        if (!netObj.IsOwner) return;

        localPlayer = other.gameObject;
        Debug.Log($"localPlayer named: {localPlayer.GetComponent<NetworkObject>().OwnerClientId}");
        interactPrompt = localPlayer.GetComponent<InteractPrompt>();

        interactPrompt?.ToggleInteractPrompt("E", "enter cart");

        playerStats = localPlayer.GetComponent<PlayerStats>();

        playerStats.nearCart = true;

        playerInput = localPlayer.GetComponent<PlayerInput>();
        characterController = localPlayer.GetComponent<CharacterController>();
        playerMovement = localPlayer.GetComponent<PlayerMovement>();
        playerAnimator = localPlayer.GetComponent<Animator>();
        inputManager = localPlayer.GetComponent<PlayerInputManager>();
        firstPersonMouseLook = localPlayer.GetComponent<MouseLook>();
        cartFollowLook = localPlayer.GetComponent<CameraManager>().cartCamera.GetComponent<CameraFollow>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == localPlayer)
        {
            interactPrompt?.ToggleInteractPrompt();

            playerAnimator = null;
            inputManager = null;
            interactPrompt = null;
            playerInput = null;

            playerStats.nearCart = false;
            localPlayer = null;
            if (localPlayer == null)
            {
                Debug.Log("localPlayer is null");
            }
        }
    }

    void Update()
    {
        if (playerInput == null) return;

        if (playerInput.actions["Interact"].WasPressedThisFrame())
        {
            if (playerStats.nearCart && !playerStats.inCart)
                EnterCart();
            else if (playerStats.inCart)
                ExitCart();
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

        int seatIndex = playersInCart;
        if (seatIndex < 0 || seatIndex >= seatPositions.Length) return;

        playerNetObj.TrySetParent(cartNetObj);
        playerNetObj.transform.SetPositionAndRotation(seatPositions[seatIndex].position, seatPositions[seatIndex].rotation);

        playersInCart += 1;

        EnterCartClientRpc(playerId, seatIndex);

        playerNetObj.GetComponent<NetworkTransform>().enabled = false;

        
    }

    [ServerRpc(RequireOwnership = false)]
    void ExitCartServerRpc(ulong playerId)
    {
        var playerNetObj = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerId);
        if (playerNetObj == null) return;

        playerNetObj.transform.SetParent(null);

        int seatIndex = playersInCart; 
        playersInCart -= 1;

        ExitCartClientRpc(playerId, seatIndex);

        playerNetObj.GetComponent<NetworkTransform>().enabled = true;

    }


    [ClientRpc]
    void EnterCartClientRpc(ulong playerId, int seatIndex)
    {
        GameObject player = null;

        if (NetworkManager.Singleton.IsServer)
        {
            player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerId)?.gameObject;
        }
        else
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out var client))
                player = client.PlayerObject?.gameObject;
        }

        if (player == null)
        {
            Debug.LogWarning($"EnterCartClientRpc: couldn't find player object for client {playerId}");
            return;
        }

        player.transform.position = seatPositions[seatIndex].position;
        player.transform.rotation = seatPositions[seatIndex].rotation;

        Debug.Log($"Players in cart (entercartClientRPC) = {playersInCart}");

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

            if (player.transform.position == seatPositions[0].position)
                inputManager?.EnableCartInputMap();

            playerStats.inCart = true;

            cameraManager.HandleCartStateChanged(true);
        }
        player.GetComponent<NetworkTransform>().enabled = false;
        AssignCarOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ClientRpc]
    void ExitCartClientRpc(ulong playerId, int seatIndex)
    {
        GameObject player = null;

        if (NetworkManager.Singleton.IsServer)
        {
            player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerId)?.gameObject;
        }
        else
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out var client))
                player = client.PlayerObject?.gameObject;
        }

        if (player == null)
        {
            Debug.LogWarning($"EnterCartClientRpc: couldn't find player object for client {playerId}");
            return;
        }

        player.transform.position = seatPositions[seatIndex].position + seatPositions[seatIndex].right * 2f;
        player.transform.rotation = Quaternion.Euler(0f, seatPositions[seatIndex].rotation.eulerAngles.y + 90f, 0f);

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
            playerStats.inCart = false;


            inputManager?.DisableCartInputMap();

            cameraManager.HandleCartStateChanged(false);
        }
        
        player.GetComponent<NetworkTransform>().enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AssignCarOwnershipServerRpc(ulong clientId)
    {
        NetworkObject carNetObj = GetComponent<NetworkObject>();
        carNetObj.ChangeOwnership(clientId);
    }



}