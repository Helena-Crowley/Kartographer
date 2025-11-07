using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Netcode.Components;

public class GarageDoor : NetworkBehaviour
{
    [SerializeField] private Image percentageSlider;
    [SerializeField] private InputActionReference interact;
    [SerializeField] private AudioClip chargeUp;
    [SerializeField] private List<Transform> spawnPoints;

    private GameObject localPlayer;
    private bool playerInTrigger = false;
    private float holdTime = 0f;
    private float requiredHoldTime = 3f; // seconds to complete
    private AudioSource audioSource;
    private bool canPlaySound = true;

    void Start() => percentageSlider.gameObject.SetActive(false);

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        localPlayer = other.gameObject;
        playerInTrigger = true;

        var prompt = localPlayer.GetComponentInChildren<InteractPrompt>();
        if (prompt != null)
            prompt.ToggleInteractPrompt("E", "begin", true);

        ResetProgress();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = false;

        var prompt = localPlayer.GetComponentInChildren<InteractPrompt>();
        if (prompt != null)
            prompt.ToggleInteractPrompt();

        localPlayer = null;
        ResetProgress();
    }

    private void Update()
    {
        if (!playerInTrigger) return;

        if (interact.action.IsPressed())
        {
            if (canPlaySound)
            {
                audioSource = SoundManager.Instance.PlayLoopingSound(chargeUp, transform.position, .3f);
                canPlaySound = false;
            }
            percentageSlider.gameObject.SetActive(true);
            holdTime += Time.deltaTime;
            float progress = Mathf.Clamp01(holdTime / requiredHoldTime);
            percentageSlider.fillAmount = progress;

            if (holdTime >= requiredHoldTime)
            {
                canPlaySound = false;

                if (NetworkManager.Singleton.IsServer)
                {
                    ulong clientId = localPlayer.GetComponent<NetworkObject>().OwnerClientId;
                    SpawnPlayerAtRandom(clientId);
                    playerInTrigger = false;
                }
                else
                {
                    MovePlayerServerRpc();
                }


                ResetProgress();
            }
        }
        else if (interact.action.WasReleasedThisFrame())
        {
            ResetProgress();
        }
    }

    private void ResetProgress()
    {
        canPlaySound = true;
        SoundManager.Instance.StopSound(audioSource);
        holdTime = 0f;
        if (percentageSlider != null)
            percentageSlider.fillAmount = 0f;
        percentageSlider.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void MovePlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        SpawnPlayerAtRandom(clientId);
    }

    private void SpawnPlayerAtRandom(ulong clientId)
    {
        if (!IsServer) return;
        if (spawnPoints == null || spawnPoints.Count == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        playerObject.gameObject.GetComponent<CharacterController>().enabled = false;

        if (playerObject != null)
        {
            Debug.Log("[SpawnPlayerAtRandom()]moving player to " + point.position);
            // For owner authority: tell the client to move themselves
            TeleportPlayerClientRpc(point.position, point.rotation,
                new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                });
        }

        playerObject.gameObject.GetComponent<CharacterController>().enabled = true;
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(Vector3 position, Quaternion rotation, ClientRpcParams rpcParams = default)
    {
        Debug.Log("[SpawnPlayerAtRandom()]moving player to " + position);
        // This runs on the target client who owns the player
        if (NetworkManager.Singleton.LocalClient.PlayerObject != null)
        {
            var player = NetworkManager.Singleton.LocalClient.PlayerObject;
            var netTransform = player.GetComponent<NetworkTransform>();

            if (netTransform != null)
                netTransform.Teleport(position, rotation, Vector3.one);
            else
                player.transform.SetPositionAndRotation(position, rotation);
        }
    }

}

