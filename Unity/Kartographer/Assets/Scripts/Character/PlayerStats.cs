using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStats : MonoBehaviour
{
    public TMPro.TMP_Text healthText;
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
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{playerBaseStats.health}";
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // disable controls, play animation, respawn, etc.
    }
}
