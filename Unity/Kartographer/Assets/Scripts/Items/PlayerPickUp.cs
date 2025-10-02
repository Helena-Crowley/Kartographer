using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUp : MonoBehaviour
{
    private PickUp nearbyPickup;
    public InputActionReference pickUpAction;
    public InputActionReference dropAction;
    public GameObject pickUpPrompt;
    public Inventory playerInventory;
    public InventoryIconGenerator iconGenerator;
    public GameObject pickupPrefab;

    void Start() => pickUpPrompt.SetActive(false);

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PickUp pickup))
        {
            pickUpPrompt.SetActive(true);
            nearbyPickup = pickup;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PickUp pickup) && pickup == nearbyPickup)
        {
            pickUpPrompt.SetActive(false);
            nearbyPickup = null;
        }
    }

    void Update()
    {
        if (nearbyPickup != null && pickUpAction.action.WasPerformedThisFrame())
        {
            pickUpPrompt.SetActive(false);
            nearbyPickup.OnPickup(gameObject);
            nearbyPickup = null; // Clear reference after pickup
        }
        if (dropAction.action.WasPerformedThisFrame())
        {
            ItemData item = playerInventory.GetLastItem();
            if (item != null && pickupPrefab != null)
            {
                // Spawn the generic pickup prefab
                GameObject spawned = Instantiate(pickupPrefab, transform.position + transform.forward + Vector3.up * 2, Quaternion.identity);

                // Assign the ItemData to the spawned PickUp component
                PickUp pickUpComp = spawned.GetComponent<PickUp>();
                if (pickUpComp != null)
                {
                    pickUpComp.itemData = item;
                }

                // Remove item from inventory
                playerInventory.Remove(item);
                iconGenerator.ClearSlot(iconGenerator.GetNextAvailableSlot() - 1);
            }
        }


    }
}
