using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerPickUp : NetworkBehaviour
{
    private PickUppableItem nearbyPickup;
    public InputActionReference pickUpAction;
    public InputActionReference dropAction;
    private InteractPrompt interactPrompt;
    public Inventory playerInventory;
    public InventoryIconGenerator iconGenerator;
    public GameObject pickupPrefab;
    public AudioClip dropSoundEffect;
    public AudioClip pickUpSoundEffect;

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
            SoundManager.Instance.PlaySound(pickUpSoundEffect, transform.position, .4f, true, 2f);
        }
        if (dropAction.action.WasPerformedThisFrame())
        {
            if (IsOwner)
            {
                DropItemServerRpc();
            }

        }
    }


    [ServerRpc]
    private void DropItemServerRpc(ServerRpcParams rpcParams = default)
    {
        ItemData item = playerInventory.GetLastItem();
        if (item != null && pickupPrefab != null)
        {
            GameObject spawned = Instantiate(pickupPrefab, playerCam.transform.position + playerCam.transform.forward * 2, Quaternion.identity);

            // Initialize itemData before spawning
            PickUppableItem pickUpComponent = spawned.GetComponent<PickUppableItem>();
            if (pickUpComponent != null)
            {
                pickUpComponent.Initialize(item, ItemDatabase.Instance.GetItemIndex(item), ItemDatabase.Instance);
            }

            spawned.GetComponent<NetworkObject>().Spawn();

            Rigidbody rb = spawned.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 forward = playerCam.transform.forward;
                Vector3 throwDir = forward * 0.35f + Vector3.up * 0.35f;
                rb.AddForce(throwDir, ForceMode.Impulse);
            }

            // Remove item from inventory
            playerInventory.Remove(item);
            int slot = iconGenerator.GetNextAvailableSlot();
            if (slot != -1)
            {
                iconGenerator.ClearSlot(slot - 1);
            }
            else
            {
                iconGenerator.ClearSlot(iconGenerator.inventorySlots.Length - 1);
            }
        }
    }


    void HandleRaycast()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * interactDistance, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, pickupLayer))
        {
            if (hit.collider.GetComponentInParent<PickUppableItem>() is PickUppableItem pickup)
            {

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
