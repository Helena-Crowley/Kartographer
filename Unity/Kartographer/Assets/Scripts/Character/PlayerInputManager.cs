using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] public InputActionAsset controls;

    private InputActionMap playerMap;
    private InputActionMap carMap;

    public System.Action<bool> OnCartStateChanged;
    private bool inCart = false;

    private void Awake()
    {
        playerMap = controls.FindActionMap("Player", true);
        carMap = controls.FindActionMap("Car", true);
    }

    private void OnEnable()
    {
        playerMap.Enable();
    }

    private void OnDisable()
    {
        playerMap.Disable();
        carMap.Disable();       
    }

    public bool InCart => inCart; // provides read only access to inCart

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
