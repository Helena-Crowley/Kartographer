using UnityEngine;

public class PickUppableItem : MonoBehaviour
{
    public ItemData itemData;

    void Start()
    {
        if (itemData.prefab != null)
        {
            // Instantiate the visual mesh as a child
            GameObject meshInstance = Instantiate(itemData.prefab, transform.position, itemData.prefab.transform.rotation, transform);

            // Apply scale
            meshInstance.transform.localScale = itemData.defaultScale;

            // Get Renderer
            Renderer rend = meshInstance.GetComponent<Renderer>();

            // Adjust position so it sits on the container prefab
            if (rend != null)
            {
                meshInstance.transform.localPosition = new Vector3(0, rend.bounds.extents.y, 0);
            }
            else
            {
                meshInstance.transform.localPosition = Vector3.zero;
            }

            // Add a MeshCollider if you want the mesh itself to interact with physics
            MeshCollider meshCol = meshInstance.GetComponent<MeshCollider>();
            if (meshCol == null)
                meshCol = meshInstance.AddComponent<MeshCollider>();

            meshCol.sharedMesh = meshInstance.GetComponent<MeshFilter>().sharedMesh;
            meshCol.convex = true; // Required for Rigidbody interactions
        }

        // Ensure Rigidbody on the container is dynamic
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public void OnPickup(GameObject player)
    {
        Debug.Log($"{player.name} picked up {itemData.displayName}");
        itemData.owner = player;

        Inventory inventory = player.GetComponent<Inventory>();
        InventoryIconGenerator iconGenerator = player.GetComponent<InventoryIconGenerator>();

        if (inventory != null)
        {
            inventory.Add(itemData);

            if (iconGenerator != null)
            {
                int slotIndex = iconGenerator.GetNextAvailableSlot();
                if (slotIndex >= 0)
                {
                    iconGenerator.GenerateIcon(itemData, slotIndex);
                }
                else
                {
                    Debug.LogWarning("No available inventory slots!");
                }
            }
        }
        else
        {
            Debug.LogWarning("Player has no Inventory component!");
        }

        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f);
    }

}
