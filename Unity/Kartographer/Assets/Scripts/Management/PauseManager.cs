using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(PlayerInput))]
public class PauseManager : NetworkBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    public GameObject playerHUD;

    private PlayerInput playerInput;
    private PlayerInputManager inputManager;
    private InputAction pauseAction;
    private bool isPaused = false;

    private void Start()
    {
        // Only local owner should control pause
        if (!IsOwner)
        {
            if (pauseMenu != null) pauseMenu.SetActive(false);
            enabled = false;
            return;
        }

        playerInput = GetComponent<PlayerInput>();
        inputManager = GetComponent<PlayerInputManager>();

        PlayerUIManager.Instance.BindPlayer(this);

        // Ensure menu starts hidden
        if (pauseMenu != null) pauseMenu.SetActive(false);

        // Cache the Pause action from any map
        pauseAction = playerInput.actions["Pause"];
        pauseAction.Enable();
    }

    private void Update()
    {
        if (!IsOwner || pauseAction == null) return;
        pauseAction = playerInput.actions["Pause"];
        // Trigger toggle when Pause is pressed
        if (pauseAction.WasPerformedThisFrame())
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!IsOwner) return;

        isPaused = !isPaused;

        if (pauseMenu != null) pauseMenu.SetActive(isPaused);
        if (playerHUD != null) playerHUD.SetActive(!isPaused);

        if (isPaused)
        {
            // Switch to UI map
            playerInput.SwitchCurrentActionMap("UI");

            // Disable cart/player input
            inputManager?.DisableNonUIInput();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Restore previous map
            if (inputManager != null)
            {
                if (inputManager.InCart)
                    inputManager.EnableCartInputMap();
                else
                    inputManager.DisableCartInputMap();
            }
            else
            {
                playerInput.SwitchCurrentActionMap("Player");
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
}
