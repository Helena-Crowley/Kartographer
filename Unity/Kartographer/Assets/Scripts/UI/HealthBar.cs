using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image iconImage;      // Current icon
    public Sprite[] icons;       // 10 sprites
    public TMPro.TextMeshProUGUI healthText;

    void Start()
    {
        if (icons.Length != 10)
        {
            Debug.LogWarning("Expected 10 icons for the health bar!");
        }
        UpdateHealthBar(100);
    }

    public void UpdateHealthBar(int health)
    {
        health = Mathf.Clamp(health, 0, 100);

        // Map health 100-0 to icon index 0-9
        int index = Mathf.Clamp(9 - Mathf.FloorToInt(health / 10f), 0, 9);
        iconImage.sprite = icons[index];

        if (healthText != null)
            healthText.text = health.ToString();
    }
}
