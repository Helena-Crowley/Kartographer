using UnityEngine;
using UnityEngine.UI;

public class BatteryBar : MonoBehaviour
{
    public Image iconImage;      // Current icon
    public Image iconImageNext;  // Next icon to fade into
    public Sprite[] icons;       // 6 sprites: Full → Empty

    private int currentIndex = 0;
    private int nextIndex = 0;

    void Start()
    {
        iconImage.sprite = icons[0];
        iconImage.color = Color.white;
        iconImageNext.color = new Color(1, 1, 1, 0);
    }

    public void UpdateBatteryBar(int health)
    {
        currentIndex = GetCurrentIndex(health);
        nextIndex = Mathf.Min(currentIndex + 1, icons.Length - 1);

        iconImage.sprite = icons[currentIndex];
        iconImageNext.sprite = icons[nextIndex];

        float rangeMin = GetThresholdMin(currentIndex);
        float rangeMax = GetThresholdMax(currentIndex);

        float t = Mathf.Clamp01((rangeMax - health) / (rangeMax - rangeMin));

        iconImage.color = new Color(1, 1, 1, 1 - t);
        iconImageNext.color = new Color(1, 1, 1, t);
    }

    private int GetCurrentIndex(int health)
    {
        if (health > 83) return 0;  // Full
        if (health > 66) return 1;
        if (health > 50) return 2;
        if (health > 33) return 3;
        if (health > 16) return 4;
        return 5;                    // Empty
    }

    private float GetThresholdMin(int index)
    {
        switch (index)
        {
            case 0: return 84;
            case 1: return 67;
            case 2: return 51;
            case 3: return 34;
            case 4: return 17;
            default: return 0;
        }
    }

    private float GetThresholdMax(int index)
    {
        switch (index)
        {
            case 0: return 100;
            case 1: return 83;
            case 2: return 66;
            case 3: return 50;
            case 4: return 33;
            default: return 16;
        }
    }
}
