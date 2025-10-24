using UnityEngine;
using Unity.Netcode;

public class PlayerStats : NetworkBehaviour
{
    [Header("UI References")]
    public HealthBar healthBar;
    public StaminaBar staminaBar;

    [Header("Stats")]
    public PlayerBaseStatsSO playerBaseStats;
    public float staminaDrainRate = 5f;
    public int staminaDrainSpeed = 10;

    public float currentStamina;
    private float maxStamina;
    public int currentHealth;

    [HideInInspector] public bool nearCart = false;
    [HideInInspector] public bool inCart = false;

    public override void OnNetworkSpawn()
    {
        currentHealth = playerBaseStats.health;
        currentStamina = playerBaseStats.stamina;
        maxStamina = playerBaseStats.stamina;

        UpdateHealthUI();
        UpdateStaminaUI();
    }

    [ServerRpc]
    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        UpdateHealthClientRpc(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        // Update UI only for local player
        if (IsOwner)
        {
            currentHealth = newHealth;
            UpdateHealthUI();
        }
    }

    public void DrainStamina()
    {
        if (!IsOwner) return;

        currentStamina -= staminaDrainRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }

    public void RegainStamina()
    {
        if (!IsOwner) return;

        currentStamina += staminaDrainRate / 2 * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }

    private void UpdateHealthUI()
    {
        healthBar.UpdateHealthBar(currentHealth);
    }

    private void UpdateStaminaUI()
    {
        staminaBar.UpdateStaminaBar(currentStamina);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
    }
}
