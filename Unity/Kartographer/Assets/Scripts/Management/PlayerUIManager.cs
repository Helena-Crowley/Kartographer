using System.ComponentModel;
using LRS;
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

    [Header("InteractPrompt")]
    public GameObject interactGO;
    public TMP_Text interactText;

    [Header("InventoryIconGenerator")]
    public Image[] inventorySlots;       // Your 5-box inventory UI
    public Sprite offHandIcon;

    [Header("DistanceText")]
    public Transform stormTransform;
    public TMP_Text distanceText;

    [Header("ScannerUI")]
    public Image scannerSlider;
    public TMP_Text scannerCompletedText;
    public TMP_Text scanningZoneText;
    // public MinimapFogController minimapFogController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
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

    public void BindPlayer(ScannerUI scannerUI)
    {
        scannerSlider.fillAmount = 0;
        scannerUI.percentageSlider = scannerSlider;
        scannerUI.completedText = scannerCompletedText;
        scannerUI.scanningZoneText = scanningZoneText;
    }

    // public void BindPlayer(FogPainter fogPainter)
    // {
    //     minimapFogController.minimapCamera = fogPainter.minimapCamera;
    // }
}
