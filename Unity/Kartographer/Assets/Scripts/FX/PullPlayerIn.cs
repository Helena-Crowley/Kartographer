using UnityEngine;
using Unity.Netcode;

public class PullPlayerIn : NetworkBehaviour
{
    public Transform pullTarget;
    public float pullDuration = .5f;
    public DoorInteract door;
    public AudioSource tvSound;
    public AudioSource boomSound;
    public GameObject fxGo;
    public VideoSwitcher videoSwitcher;

    void OnTriggerEnter(Collider other)
    {

        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            var networkObj = other.GetComponent<NetworkObject>();
            if (networkObj != null)
            {
                Debug.Log($"Sending RPC to client {networkObj.OwnerClientId}");

                var rpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new[] { networkObj.OwnerClientId }
                    }
                };

                PullPlayerClientRpc(pullTarget.position, pullDuration, rpcParams);

                boomSound.Play();
            }
            else
            {
                Debug.LogWarning("No NetworkObject on player!");
            }
        }
    }


    [ClientRpc]
    void PullPlayerClientRpc(Vector3 targetPos, float duration, ClientRpcParams rpcParams = default)
    {
        Debug.Log("PullPlayerClientRpc received!");
        var puller = FindAnyObjectByType<PlayerPullHandler>();
        if (puller != null)
        {
            Debug.Log("Found PlayerPullHandler, starting pull...");
            puller.StartPull(targetPos, duration, door, tvSound, fxGo, videoSwitcher);
        }
        else
        {
            Debug.LogWarning("No PlayerPullHandler found in scene!");
        }
    }

}
