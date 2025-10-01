using UnityEngine;

public class PickUp : MonoBehaviour
{
    public ItemData itemData;
    public InventoryIconGenerator iconGenerator;

    void Start()
    {
        if (itemData.prefab != null)
        {
            // Instantiate the mesh as a child
            GameObject meshInstance = Instantiate(itemData.prefab, transform.position, itemData.prefab.transform.rotation, transform);

            // Apply scale first
            meshInstance.transform.localScale = itemData.defaultScale;

            // Get the renderer and bounds **after scaling**
            Renderer rend = meshInstance.GetComponent<Renderer>();
            if (rend != null)
            {
                // Move it up so it sits flat on the surface
                meshInstance.transform.localPosition = new Vector3(0, rend.bounds.extents.y, 0);
            }
            else
            {
                meshInstance.transform.localPosition = Vector3.zero; // fallback
            }
        }
    }

    public void OnPickup(GameObject player)
    {
        Debug.Log($"{player.name} picked up {itemData.displayName}");
        // Add to inventory system using itemData
        Inventory inventory = player.GetComponent<Inventory>();
        if (inventory != null)
        {
            iconGenerator.GenerateIcon(itemData, 1);
            inventory.Add(itemData);
        }
        else
        {
            Debug.LogWarning("Player has no Inventory component!");
        }
        // Disable the pickup instead of destroying it immediately
        gameObject.SetActive(false);

        // Optionally, destroy after a frame or delay
        Destroy(gameObject, 0.1f);
    }
}

