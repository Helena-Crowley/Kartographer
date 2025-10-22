using UnityEngine;
using UnityEngine.InputSystem;


public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;

    [SerializeField] private InputActionReference pauseAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        pauseAction.action.performed += OnPausePressed;
        pauseAction.action.Enable();

    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePressed;
        pauseAction.action.Disable();

    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        // Toggle local pause UI
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused = true;

        // Stop player input, not time
        var player = FindLocalPlayerMovement();
        var mouse = FindLocalMouseLook();

        if (player != null && mouse != null)
        {
            player.enabled = false;
            mouse.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        isPaused = false;

        var player = FindLocalPlayerMovement();
        var mouse = FindLocalMouseLook();

        if (player != null && mouse != null)
        {
            player.enabled = true;
            mouse.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private MonoBehaviour FindLocalPlayerMovement()
    {
        foreach (var player in Object.FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (player.IsOwner)
                return player;
        }
        return null;
    }

    private MonoBehaviour FindLocalMouseLook()
    {
        foreach (var player in Object.FindObjectsByType<MouseLook>(FindObjectsSortMode.None))
        {
            if (player.IsOwner)
                return player;
        }
        return null;
    }
}
