using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using UnityEngine.InputSystem;

public class OutPostTerminal : NetworkBehaviour
{
    [Header("Loading Screen UI")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingBar;

    [Header("Teleport Target")]
    [SerializeField] private SpawnData spawnPoints;

    [Header("Input")]
    [SerializeField] private InputActionReference interact;

    private bool playerInTrigger = false;
    private Transform teleportDestination;
    private GameObject localPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        localPlayer = other.gameObject;
        playerInTrigger = true;

        var prompt = localPlayer.GetComponentInChildren<InteractPrompt>();
        if (prompt != null)
            prompt.ToggleInteractPrompt("E", "go to Outpost");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = false;

        var prompt = localPlayer.GetComponentInChildren<InteractPrompt>();
        if (prompt != null)
            prompt.ToggleInteractPrompt();

        localPlayer = null;
    }

    private void Update()
    {
        if (playerInTrigger && interact.action.WasPressedThisFrame())
        {
            BeginTravel();
        }
    }

    public void BeginTravel()
    {
        if (IsOwner)
        {
            int index = Random.Range(0, spawnPoints.positions.Length);
            Vector3 newPosition = spawnPoints.positions[index];
            Debug.Log($"index = {index}");
            Debug.Log($"spawnpoint position = {newPosition}");

            StartCoroutine(TravelCoroutine(newPosition));
        }
    }


    private IEnumerator TravelCoroutine(Vector3 newPosition)
    {
        loadingScreen.SetActive(true);
        loadingBar.value = 0f;

        float elapsed = 0f;
        float duration = 2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            loadingBar.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        TeleportServerRpc(newPosition);

        yield return new WaitForSeconds(0.3f);
        loadingScreen.SetActive(false);
    }


    [ServerRpc(RequireOwnership = false)]
    private void TeleportServerRpc(Vector3 newPos, ServerRpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        NetworkObject player = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;

        player.transform.position = newPos;
        TeleportClientRpc(senderClientId, newPos);
    }

    [ClientRpc]
    private void TeleportClientRpc(ulong clientId, Vector3 newPos)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform.position = newPos;
        }
    }

}
