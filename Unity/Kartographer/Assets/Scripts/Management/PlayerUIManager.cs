using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance;

    [Header("HUD References")]
    public GameObject playerHUD;
    public HealthBar healthBar;
    public StaminaBar staminaBar;

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
}
