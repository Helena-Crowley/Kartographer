using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStats : MonoBehaviour
{
    public HealthBar healthBar;
    public StaminaBar staminaBar;
    public float staminaDrainRate = 5f; // stamina per second
    public float currentStamina = 100f;
    public float maxStamina;
    public int staminaDrainSpeed = 10; // Higher is slower drain
    public PlayerBaseStatsSO playerBaseStats;
    private int currentHealth;

    void Start()
    {
        currentHealth = playerBaseStats.health;
        currentStamina = playerBaseStats.stamina;
        maxStamina = playerBaseStats.stamina;
        UpdateStaminaUI();
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void DrainStamina()
    {
        currentStamina -= staminaDrainRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }

    public void RegainStamina()
    {
        currentStamina += staminaDrainRate / 2 * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }


    private void UpdateStaminaUI()
    {
        // Implement stamina drain logic if needed
        staminaBar.UpdateStaminaBar(currentStamina);
    }


    private void UpdateHealthUI()
    {
        healthBar.UpdateHealthBar(currentHealth);
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // disable controls, play animation, respawn, etc.
    }
}
