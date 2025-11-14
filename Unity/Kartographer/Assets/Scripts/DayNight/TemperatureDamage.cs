using TMPro;
using UnityEngine;

public class TemperatureDamage : MonoBehaviour
{
    [SerializeField] private TMP_Text temperatureText;
    [SerializeField] private DayNightCycle dayNightCycle;


    // Update is called once per frame
    void Update()
    {
        temperatureText.text = dayNightCycle.currentTemperature.ToString("F0") + "\u00B0C";
    }
}
