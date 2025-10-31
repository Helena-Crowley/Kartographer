using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputManager : NetworkBehaviour
{
    //
    //PER PLAYER NOT A SINGLETON
    //
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    

    private InputActionMap playerMap;
    private InputActionMap carMap;
    private InputActionMap currentActionMap;

    private bool inCart = false;

    /// <summary>
    /// Invoked whenever the player enters or exits a cart.
    /// true = in cart, false = on foot
    /// </summary>
    public System.Action<bool> OnCartStateChanged;

    public InputActionMap GetCurrentInputMapping()
    {
        return currentActionMap;
    }

    private void Awake()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        playerMap = playerInput.actions.FindActionMap("Player", true);
        carMap = playerInput.actions.FindActionMap("Car", true);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // Enable player input for local owner only
            playerMap.Enable();
            currentActionMap = playerMap;
            carMap.Disable();
        }
        else
        {
            // Disable input for non-owners
            playerMap.Disable();
            carMap.Disable();
        }
    }

    private void OnEnable()
    {
        if (IsOwner)
        {
            playerMap.Enable();
            currentActionMap = playerMap;
            carMap.Disable();
        }
    }

    private void OnDisable()
    {
        if (IsOwner)
        {
            playerMap.Disable();
            carMap.Disable();
        }
    }

    public bool InCart => inCart;
    public PlayerInput PlayerInput => playerInput;

    /// <summary>Switches to car input for the local owner and updates cart state.</summary>
    public void EnableCartInputMap()
    {
        if (!IsOwner) return;

        carMap.Enable();
        currentActionMap = carMap;
        playerMap.Disable();
        inCart = true;
        OnCartStateChanged?.Invoke(true);
    }

    /// <summary>Switches back to player input for the local owner and updates cart state.</summary>
    public void DisableCartInputMap()
    {
        if (!IsOwner) return;

        playerMap.Enable();
        currentActionMap = playerMap;
        carMap.Disable();
        inCart = false;
        OnCartStateChanged?.Invoke(false);
    }

    public void DisableNonUIInput()
    {
        playerMap.Disable();
        currentActionMap = null;
        carMap.Disable();
    }
}
