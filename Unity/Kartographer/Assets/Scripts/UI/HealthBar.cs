using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image iconImage;      // Current icon
    public Image iconImageNext;  // Next icon to fade into
    public Sprite[] icons;

    private int currentIndex = 0;
    private int nextIndex = 0;

    void Start()
    {
        iconImage.sprite = icons[0];
        iconImage.color = Color.white;
        iconImageNext.color = new Color(1,1,1,0);
    }

    public void UpdateHealthBar(int health)
    {
        // Determine the index for current and next icon based on thresholds
        currentIndex = GetCurrentIndex(health);
        nextIndex = Mathf.Min(currentIndex + 1, icons.Length - 1);

        iconImage.sprite = icons[currentIndex];
        iconImageNext.sprite = icons[nextIndex];

        // Get the health range for this threshold
        float rangeMin = GetThresholdMin(currentIndex);
        float rangeMax = GetThresholdMax(currentIndex);

        // Calculate percentage of fade (0 = start of interval, 1 = end of interval)
        float t = Mathf.Clamp01((rangeMax - health) / (rangeMax - rangeMin));

        iconImage.color = new Color(1,1,1,1 - t);
        iconImageNext.color = new Color(1,1,1,t);
    }

    private int GetCurrentIndex(int health)
    {
        if (health > 90) return 0;
        if (health > 75) return 1;
        if (health > 50) return 2;
        if (health > 25) return 3;
        return 4;
    }

    private float GetThresholdMin(int index)
    {
        switch(index)
        {
            case 0: return 91;  // Above 90
            case 1: return 76;  // 76–90
            case 2: return 51;  // 51–75
            case 3: return 26;  // 26–50
            default: return 0;  // 0–25
        }
    }

    private float GetThresholdMax(int index)
    {
        switch(index)
        {
            case 0: return 100;
            case 1: return 90;
            case 2: return 75;
            case 3: return 50;
            default: return 25;
        }
    }
}
