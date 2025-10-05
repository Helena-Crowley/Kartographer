using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUp : MonoBehaviour
{
    private PickUp nearbyPickup;
    public InputActionReference pickUpAction;
    public InputActionReference dropAction;
    private InteractPrompt interactPrompt;
    public Inventory playerInventory;
    public InventoryIconGenerator iconGenerator;
    public GameObject pickupPrefab;

    public float interactDistance = 3f;
    public LayerMask pickupLayer;

    [SerializeField] Camera playerCam;

    void Start()
    {
        interactPrompt = GetComponent<InteractPrompt>();
        interactPrompt.ToggleInteractPrompt();
    }

    void Update()
    {
        HandleRaycast();

        if (nearbyPickup != null && pickUpAction.action.WasPerformedThisFrame())
        {
            interactPrompt.ToggleInteractPrompt();
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
                int slot = iconGenerator.GetNextAvailableSlot();
                if (slot != -1)
                {
                    iconGenerator.ClearSlot(slot - 1);
                }
                else if (slot == -1)
                {
                    iconGenerator.ClearSlot(iconGenerator.inventorySlots.Length - 1);
                }
                else
                {
                    Debug.LogWarning("No PickUp component found on the pickup prefab.");
                }
            }
        }



    }
    void HandleRaycast()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * interactDistance, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, pickupLayer))
        {
            if (hit.collider.GetComponentInParent<PickUp>() is PickUp pickup)
            {
                Debug.Log($"Looking at pickup: {pickup.itemData.displayName}");
                // Show prompt if looking at a pickup
                if (nearbyPickup != pickup)
                {
                    nearbyPickup = pickup;
                    interactPrompt.ToggleInteractPrompt("F", "pick up");
                }
                return;
            }
        }
        // Nothing hit or no PickUp
        if (nearbyPickup != null)
        {
            nearbyPickup = null;
            interactPrompt.ToggleInteractPrompt();
        }
    }
}
