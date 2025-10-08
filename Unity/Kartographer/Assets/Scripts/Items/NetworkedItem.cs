using Unity.Netcode;
using UnityEngine;

public class NetworkedItem : NetworkBehaviour
{
    public ItemData itemData;

    void Start()
    {
        if (itemData.prefab != null)
        {
            GameObject meshInstance = Instantiate(itemData.prefab, transform.position, itemData.prefab.transform.rotation, transform);
            meshInstance.transform.localScale = itemData.defaultScale;

            Renderer rend = meshInstance.GetComponent<Renderer>();
            meshInstance.transform.localPosition = rend != null ? new Vector3(0, rend.bounds.extents.y, 0) : Vector3.zero;

            MeshCollider meshCol = meshInstance.GetComponent<MeshCollider>();
            if (meshCol == null) meshCol = meshInstance.AddComponent<MeshCollider>();
            meshCol.sharedMesh = meshInstance.GetComponent<MeshFilter>().sharedMesh;
            meshCol.convex = true;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public void TryPickUp(GameObject player)
    {
        if (IsServer)
            PickUpServer(player);
        else
            PickUpServerRpc(player.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpServerRpc(ulong playerId)
    {
        var playerObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerId].gameObject;
        PickUpServer(playerObj);
    }

    private void PickUpServer(GameObject player)
    {
        Inventory inventory = player.GetComponent<Inventory>();
        InventoryIconGenerator iconGenerator = player.GetComponent<InventoryIconGenerator>();

        if (inventory != null)
        {
            inventory.Add(itemData);

            if (iconGenerator != null)
            {
                int slotIndex = iconGenerator.GetNextAvailableSlot();
                if (slotIndex >= 0)
                    iconGenerator.GenerateIcon(itemData, slotIndex);
            }
        }

        // Remove for everyone
        GetComponent<NetworkObject>().Despawn();
    }
}
