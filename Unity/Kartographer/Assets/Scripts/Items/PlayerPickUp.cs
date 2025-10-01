using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUp : MonoBehaviour
{
    private PickUp nearbyPickup;
    public InputActionReference pickUpAction;
    public GameObject pickUpPrompt;

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

    void Update() {
        if(nearbyPickup != null && pickUpAction.action.WasPerformedThisFrame()) {
            pickUpPrompt.SetActive(false);
            nearbyPickup.OnPickup(gameObject);
            nearbyPickup = null; // Clear reference after pickup
        }
    }
}
