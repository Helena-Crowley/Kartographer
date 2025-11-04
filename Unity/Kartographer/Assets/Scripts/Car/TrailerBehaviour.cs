// using Unity.VisualScripting;
// using UnityEngine;

// public class TrailerBehaviour : MonoBehaviour
// {
//     private Rigidbody rb;
//     private void OnTriggerEnter(Collider other)
//     {
//         GameObject itemMesh = other.gameObject;
//         if (itemMesh.transform.parent.tag == "Scrap")
//         {
//             rb = itemMesh.GetComponentInParent<Rigidbody>();
//             if (rb != null)
//             {
//                 rb.isKinematic = true;
//                 Debug.Log("enabled is kinematic");
//             }
//             else
//             {
//                 Debug.Log("rb from sceap was null");
//             }
//         }
//     }
// }
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TrailerBehaviour : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // get the NetworkObject on the scrap's rigidbody root (safer)
        var attachedRb = other.attachedRigidbody;
        if (attachedRb == null) return;

        var scrapNetObj = attachedRb.GetComponent<NetworkObject>();
        if (scrapNetObj == null) return; // not a networked scrap

        // Call server to attach (allow non-owners to call)
        RequestAttachToTrailerServerRpc(scrapNetObj.NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestAttachToTrailerServerRpc(ulong scrapNetworkId, ServerRpcParams rpcParams = default)
    {
        // Ensure server has the NetworkObject
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(scrapNetworkId, out var scrapNetObj))
        {
            Debug.LogWarning("Server: scrap not found in SpawnedObjects.");
            return;
        }

        // Optional: wait or check if scrap is settled; here we parent immediately on server
        // Ensure this trailer is also a NetworkObject and spawned
        var myNetObj = GetComponent<NetworkObject>();
        if (myNetObj == null || !myNetObj.IsSpawned)
        {
            Debug.LogWarning("Trailer has no spawned NetworkObject; cannot parent.");
            return;
        }

        // TrySetParent the scrap's NetworkObject under this trailer's NetworkObject
        bool ok = scrapNetObj.TrySetParent(myNetObj);
        Debug.Log($"Server: TrySetParent result = {ok} for {scrapNetObj.name}");
    }
}

