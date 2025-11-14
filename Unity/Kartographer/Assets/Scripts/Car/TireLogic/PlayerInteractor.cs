using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    public Transform handPosition;
    [SerializeField] private GameObject cam;
    [SerializeField] private InteractPrompt interactPrompt;

    [Header("Settings")]
    [SerializeField] private float interactDistance = 3f;

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference dropAction;

    [HideInInspector] public bool holdingItem;

    private IInteractable currentLookedAtInteractable;
    private WheelSlot currentLookedAtSlot;
    private GameObject heldObject;
    private IInteractable heldInteractable;

    [SerializeField] private AudioClip attachSound;




    // Track whether we were showing a prompt last frame
    private bool wasShowingPrompt = false;

    private void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    private void HandleRaycast()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        bool foundPromptTarget = false;

        foreach (var hit in hits)
        {
            // Interactable
            if (hit.collider.TryGetComponent(out IInteractable interactable) && !holdingItem)
            {
                currentLookedAtInteractable = interactable;
                currentLookedAtSlot = null;
                heldObject = interactable is Component c ? c.gameObject : null;
                interactPrompt.ToggleInteractPrompt("F", "Pick Up");
                foundPromptTarget = true;
                wasShowingPrompt = true;
                break;
            }

            WheelSlot slot;

            // Wheel slot
            if (holdingItem && hit.collider.TryGetComponent(out slot) && !slot.isOccupied)
            {
                slot.CheckOccupiedStatus();
                currentLookedAtSlot = slot;
                currentLookedAtInteractable = null;
                interactPrompt.ToggleInteractPrompt("Q", "Put On");
                foundPromptTarget = true;
                wasShowingPrompt = true;
                break;
            }
            else if (holdingItem && hit.collider.TryGetComponent(out slot))
            {
                slot.CheckOccupiedStatus();
            }
        }

        // Only hide prompt ONCE when we stop looking at something
        if (!foundPromptTarget)
        {
            currentLookedAtInteractable = null;
            currentLookedAtSlot = null;

            // Only call HidePrompt once when transitioning from showing to not showing
            if (wasShowingPrompt)
            {
                interactPrompt.HidePrompt();
                wasShowingPrompt = false;
            }
        }
    }

    private void HandleInput()
    {
        // Pick up interactable
        if (interactAction.action.WasPressedThisFrame() && currentLookedAtInteractable != null)
        {
            heldInteractable = currentLookedAtInteractable;
            heldObject = (currentLookedAtInteractable as Component).gameObject;
            heldInteractable.Interact(this);
            holdingItem = true;
        }

        // Drop or place
        if (dropAction.action.WasPressedThisFrame() && holdingItem)
        {
            if (currentLookedAtSlot != null)
            {
                // Snap to slot
                heldObject.transform.position = currentLookedAtSlot.mountPoint.position;
                heldObject.transform.rotation = currentLookedAtSlot.mountPoint.rotation;

                heldObject.transform.parent = currentLookedAtSlot.transform;

                SoundManager.Instance.PlaySound(attachSound, currentLookedAtSlot.transform.position, "SFX", .2f, true);


                currentLookedAtSlot.isOccupied = true;

                holdingItem = false;
                heldInteractable = null;
                heldObject = null;

            }
            else if (heldInteractable != null)
            {
                // Drop in front
                heldInteractable.Drop(this);
                holdingItem = false;
                heldInteractable = null;
                heldObject = null;
            }
        }
    }
}