using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Static instance accessible from anywhere
    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionAsset controls;

    private InputActionMap playerMap;
    private InputActionMap carMap;

    public static event System.Action<bool> OnCartStateChanged;
    private bool inCart = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerMap = controls.FindActionMap("Player", true);
        carMap = controls.FindActionMap("Car", true);
    }

    private void OnEnable()
    {
        playerMap.Enable();
    }

    public bool InCart => inCart;

    public void EnterCart()
    {
        EnableCar();
        inCart = true;
        OnCartStateChanged?.Invoke(inCart);
    }

    public void ExitCart()
    {
        EnablePlayer();
        inCart = false;
        OnCartStateChanged?.Invoke(inCart);
    }

    public void EnablePlayer()
    {
        carMap.Disable();
        playerMap.Enable();
    }

    private void EnableCar()
    {
        playerMap.Disable();
        carMap.Enable();
    }
}
