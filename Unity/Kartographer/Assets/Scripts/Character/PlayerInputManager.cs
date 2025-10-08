// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerInputManager : MonoBehaviour
// {
//     [SerializeField] public InputActionAsset controls;

//     private InputActionMap playerMap;
//     private InputActionMap carMap;

//     public System.Action<bool> OnCartStateChanged;
//     private bool inCart = false;

//     private void Awake()
//     {
//         playerMap = controls.FindActionMap("Player", true);
//         carMap = controls.FindActionMap("Car", true);
//     }

//     private void OnEnable()
//     {
//         playerMap.Enable();
//     }

//     private void OnDisable()
//     {
//         playerMap.Disable();
//         carMap.Disable();       
//     }

//     public bool InCart => inCart; // provides read only access to inCart

//     public void EnterCart()
//     {
//         EnableCar();
//         inCart = true;
//         OnCartStateChanged?.Invoke(inCart);
//     }

//     public void ExitCart()
//     {
//         EnablePlayer();
//         inCart = false;
//         OnCartStateChanged?.Invoke(inCart);
//     }

//     public void EnablePlayer()
//     {
//         carMap.Disable();
//         playerMap.Enable();
//     }

//     private void EnableCar()
//     {
//         playerMap.Disable();
//         carMap.Enable();
//     }
// }
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerInputManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;

    private InputActionMap playerMap;
    private InputActionMap carMap;

    private bool inCart = false;

    public System.Action<bool> OnCartStateChanged;

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
            EnablePlayer();
            Debug.Log("Local player input enabled for " + gameObject.name);
        }
        else
        {
            // Disable car & player maps just to be safe
            playerMap.Disable();
            carMap.Disable();
        }
    }


    private void OnEnable()
    {
        // Only enable input for the local owner
        if (IsOwner)
            EnablePlayer();
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

    /// <summary>Switches to car input for the local owner.</summary>
    public void EnterCart()
    {
        if (!IsOwner) return;

        carMap.Enable();
        playerMap.Disable();
        inCart = true;
        OnCartStateChanged?.Invoke(inCart);
    }

    /// <summary>Switches back to player input for the local owner.</summary>
    public void ExitCart()
    {
        if (!IsOwner) return;

        playerMap.Enable();
        carMap.Disable();
        inCart = false;
        OnCartStateChanged?.Invoke(inCart);
    }

    /// <summary>Enable player controls only for the local owner.</summary>
    public void EnablePlayer()
    {
        if (!IsOwner) return;

        playerMap.Enable();
        carMap.Disable();
    }

    /// <summary>Enable car controls only for the local owner.</summary>
    public void EnableCar()
    {
        if (!IsOwner) return;

        carMap.Enable();
        playerMap.Disable();
    }
}
