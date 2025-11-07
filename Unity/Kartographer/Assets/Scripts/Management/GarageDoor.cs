// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.UI;
// using Unity.Netcode;
// using System.Collections.Generic;

// public class GarageDoor : NetworkBehaviour
// {
//     [SerializeField] private Image percentageSlider;
//     [SerializeField] private InputActionReference interact;
//     [SerializeField] private AudioClip chargeUp;
//     [SerializeField] private List<Transform> spawnPoints;

//     private GameObject localPlayer;
//     private bool playerInTrigger = false;
//     private float holdTime = 0f;
//     private float requiredHoldTime = 3f; // seconds to complete
//     private AudioSource audioSource;
//     private bool canPlaySound = true;

//     void Start() => percentageSlider.gameObject.SetActive(false);

//     private void OnTriggerEnter(Collider other)
//     {
//         if (!other.CompareTag("Player")) return;

//         localPlayer = other.gameObject;
//         playerInTrigger = true;

//         var prompt = localPlayer.GetComponentInChildren<InteractPrompt>();
//         if (prompt != null)
//             prompt.ToggleInteractPrompt("E", "open", true);

//         ResetProgress();
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (!other.CompareTag("Player")) return;

//         playerInTrigger = false;

//         var prompt = localPlayer.GetComponentInChildren<InteractPrompt>();
//         if (prompt != null)
//             prompt.ToggleInteractPrompt();

//         localPlayer = null;
//         ResetProgress();
//     }

//     private void Update()
//     {
//         if (!playerInTrigger) return;

//         if (interact.action.IsPressed())
//         {
//             if (canPlaySound)
//             {
//                 audioSource = SoundManager.Instance.PlayLoopingSound(chargeUp, transform.position, .3f);
//                 canPlaySound = false;
//             }
//             percentageSlider.gameObject.SetActive(true);
//             holdTime += Time.deltaTime;
//             float progress = Mathf.Clamp01(holdTime / requiredHoldTime);
//             percentageSlider.fillAmount = progress;

//             if (holdTime >= requiredHoldTime)
//             {
//                 canPlaySound = false;

//                 if (NetworkManager.Singleton.IsServer)
//                 {
//                     ulong clientId = localPlayer.GetComponent<NetworkObject>().OwnerClientId;
//                     SpawnPlayerAtRandom(clientId);
//                 }
//                 else
//                 {
//                     MovePlayerServerRpc();
//                 }


//                 ResetProgress();
//             }
//         }
//         else if (interact.action.WasReleasedThisFrame())
//         {
//             ResetProgress();
//         }
//     }

//     private void ResetProgress()
//     {
//         canPlaySound = true;
//         SoundManager.Instance.StopSound(audioSource);
//         holdTime = 0f;
//         if (percentageSlider != null)
//             percentageSlider.fillAmount = 0f;
//         percentageSlider.gameObject.SetActive(false);
//     }

//     [ServerRpc(RequireOwnership = false)]
//     private void MovePlayerServerRpc(ServerRpcParams rpcParams = default)
//     {
//         ulong clientId = rpcParams.Receive.SenderClientId;
//         SpawnPlayerAtRandom(clientId);
//     }

//     private void SpawnPlayerAtRandom(ulong clientId)
//     {
//         if (!IsServer) return;
//         if (spawnPoints == null || spawnPoints.Count == 0)
//         {
//             Debug.LogWarning("No spawn points assigned for GarageDoor!");
//             return;
//         }

//         Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];

//         // Get player's NetworkObject
//         NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
//         if (playerObject != null)
//         {
//             Debug.Log($"Spawning player at {point.position}");
//             playerObject.transform.SetPositionAndRotation(point.position, point.rotation);
//         }
//     }

// }
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class GarageDoor : NetworkBehaviour
{
    [Header("UI & Input")]
    [SerializeField] private Image percentageSlider;
    [SerializeField] private InputActionReference interact;

    [Header("Audio")]
    [SerializeField] private AudioClip chargeUp;
    private AudioSource audioSource;
    private bool canPlaySound = true;

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints;

    private GameObject localPlayer;
    private bool playerInTrigger = false;
    private float holdTime = 0f;
    private float requiredHoldTime = 3f; // seconds to complete

    void Start()
    {
        percentageSlider.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        localPlayer = other.gameObject;
        playerInTrigger = true;

        var prompt = localPlayer.GetComponentInChildren<InteractPrompt>();
        if (prompt != null)
            prompt.ToggleInteractPrompt("E", "open", true);

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
                audioSource = SoundManager.Instance.PlayLoopingSound(chargeUp, transform.position, 0.3f);
                canPlaySound = false;
            }

            percentageSlider.gameObject.SetActive(true);
            holdTime += Time.deltaTime;
            float progress = Mathf.Clamp01(holdTime / requiredHoldTime);
            percentageSlider.fillAmount = progress;

            if (holdTime >= requiredHoldTime)
            {
                // Player finished holding
                if (NetworkManager.Singleton.IsServer)
                {
                    // Move server-controlled objects
                    TeleportPlayerServer(localPlayer);
                }
                else
                {
                    // Ask server to teleport us
                    RequestTeleportServerRpc();
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

    // SERVER: Move player directly (if server owns object)
    private void TeleportPlayerServer(GameObject player)
    {
        if (spawnPoints == null || spawnPoints.Count == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        var netObj = player.GetComponent<NetworkObject>();
        if (netObj == null) return;

        // Use ClientRpc to let the client move itself
        MovePlayerClientRpc(point.position, point.rotation, new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { netObj.OwnerClientId } }
        });
    }

    // SERVER RPC called by client to request teleport
    [ServerRpc(RequireOwnership = false)]
    private void RequestTeleportServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        var playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        if (playerObj != null)
        {
            TeleportPlayerServer(playerObj.gameObject);
        }
    }

    // CLIENT RPC to move player
    [ClientRpc]
    private void MovePlayerClientRpc(Vector3 position, Quaternion rotation, ClientRpcParams clientRpcParams = default)
    {
        localPlayer.transform.SetPositionAndRotation(position, rotation);
    }
}
