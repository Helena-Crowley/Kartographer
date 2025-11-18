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
        if (dayNightCycle.currentTemperature < dayNightCycle.minTemperature + 17)
        {
            //and if players are not in outpost
            foreach (PlayerObj player in PlayerManager.Instance.playersInGame.Values)
            player.TakeDamage(100);
        }
    }


}
