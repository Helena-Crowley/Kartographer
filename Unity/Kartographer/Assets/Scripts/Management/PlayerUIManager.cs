using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance;

    [Header("HUD References")]
    public GameObject playerHUD;
    public HealthBar healthBar;
    public StaminaBar staminaBar;
    public GameObject interactGO;
    public TMP_Text interactText;
    public Image[] inventorySlots;       // Your 5-box inventory UI
    public Sprite offHandIcon;
    public Transform stormTransform;
    public TMP_Text distanceText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Called by the local player to hook up UI
    public void BindPlayer(PlayerObj player)
    {
        playerHUD.SetActive(true);

        player.healthBar = healthBar;
        player.staminaBar = staminaBar;

        // Initialize UI
        healthBar.UpdateHealthBar(player.currentHealth.Value);
        staminaBar.UpdateStaminaBar(player.currentStamina);
    }

    public void BindPlayer(InteractPrompt interactPrompt)
    {
        interactPrompt.interactPrompt = interactGO;
        interactPrompt.textComponent = interactText;
    }

    public void BindPlayer(InventoryIconGenerator iconGenerator)
    {
        iconGenerator.inventorySlots = inventorySlots;
        iconGenerator.offHandIcon = offHandIcon;
    }

    public void BindPlayer(DistanceText distanceTextScript)
    {
        distanceTextScript.distanceText = distanceText;
        distanceTextScript.stormTransform = stormTransform;
    }
}
