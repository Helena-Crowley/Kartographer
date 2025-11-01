using UnityEngine;
using Unity.Netcode;

public class MiniMapCameraFollow : NetworkBehaviour
{
    public Transform cameraTransform; // Assign your camera in Inspector
    public Vector3 offset = new Vector3(0, 17, 0);

    private Transform playerTransform;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerTransform = transform;
            cameraTransform.transform.SetParent(null); // Detach
        }
        else
        {
            cameraTransform.gameObject.SetActive(false); // disable non-local cameras
        }
    }

    void LateUpdate()
    {
        if (!IsOwner) return;

        // Follow position only
        cameraTransform.position = playerTransform.position + offset;
        // Don’t rotate with player — keep fixed rotation
        cameraTransform.rotation = Quaternion.Euler(90f, 0f, 0f); // top-down
    }
}
