using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStats : MonoBehaviour
{
    public HealthBar healthBar;
    public PlayerBaseStatsSO playerBaseStats;
    private int currentHealth;

    void Start()
    {
        currentHealth = playerBaseStats.health;
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
