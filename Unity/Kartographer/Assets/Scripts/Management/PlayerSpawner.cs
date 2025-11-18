using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private WendySpawner wendySpawner;

    private void Awake()
    {
        if (NetworkManager.Singleton == null)
        {
            //Debug.LogError("NetworkManager.Singleton is null!");
            return;
        }

        Debug.Log("PlayerSpawner Awake - waiting for server start");
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        Debug.Log("PlayerSpawner - Registering for connection events");
        NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null && IsServer)
            NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
    }

    private void OnConnectionEvent(NetworkManager netManager, ConnectionEventData eventData)
    {
        if (!IsServer)
        {
            Debug.Log("Non-server tried to enter player spawner");
            return;
        }

        Debug.Log("Server entered connection event");

        if (eventData.EventType == ConnectionEvent.ClientConnected)
        {
            var playerObject = NetworkManager.Singleton.ConnectedClients[eventData.ClientId].PlayerObject;
            if (playerObject != null)
            {
                Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

                Debug.Log("TeleportPlayerClentRpc called");
                TeleportPlayerClientRpc(spawn.position, spawn.rotation,
                    new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new ulong[] { eventData.ClientId }
                        }
                    });

                //send info to wendigo manager
                wendySpawner.AddPlayer(playerObject.gameObject);
                Debug.Log(playerObject.name);
                PlayerManager.Instance.RegisterPlayer(playerObject);
            }
        }
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(Vector3 position, Quaternion rotation, ClientRpcParams rpcParams = default)
    {
        Debug.Log("Teleporting player [client rpc]");
        var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (playerObject != null)
        {
            var netTransform = playerObject.GetComponent<Unity.Netcode.Components.NetworkTransform>();
            if (netTransform != null)
            {
                Debug.Log("Teleport to : " + position);
                netTransform.Teleport(position, rotation, Vector3.one);
            }
            else
            {
                Debug.Log("set position and rotation to " + position);
                playerObject.transform.SetPositionAndRotation(position, rotation);
            }
        }
    }
}