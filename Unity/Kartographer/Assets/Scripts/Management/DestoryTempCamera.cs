using Unity.Netcode;
using UnityEngine;

public class DestroyTempCamera : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer && NetworkManager.Singleton.ConnectedClients.Count == 1)
        {
            // First player just connected — destroy this object on all clients
            NetworkObject.Despawn(true);
        }
    }
}
