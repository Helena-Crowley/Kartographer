using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Image iconImage;      // Current icon
    public Sprite[] icons;       // 10 sprites
    public TMPro.TextMeshProUGUI staminaText;

    void Start()
    {
        UpdateStaminaBar(100);
    }

    public void UpdateStaminaBar(float stamina)
    {
        stamina = Mathf.Clamp(stamina, 0, 100);

        // Map health 100-0 to icon index 0-9
        int index = Mathf.Clamp(9 - Mathf.FloorToInt(stamina / 10f), 0, 9);
        iconImage.sprite = icons[index];

        staminaText.text = stamina.ToString("f0");
    }
}
