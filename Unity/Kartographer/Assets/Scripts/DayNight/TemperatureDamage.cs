using TMPro;
using UnityEngine;
using System.Collections;

public class TemperatureDamage : MonoBehaviour
{
    [SerializeField] private TMP_Text temperatureText;
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private float damageInterval = 3f; // every 3 seconds
    [SerializeField] private int damageAmount = 5;    // damage per tick

    private Coroutine damageCoroutine;

    void Update()
    {
        // Update temperature UI
        temperatureText.text = dayNightCycle.currentTemperature.ToString("F0") + "\u00B0C";

        // Start or stop the damage coroutine depending on temperature
        if (dayNightCycle.currentTemperature < dayNightCycle.minTemperature + 17) // set to 17 for build
        {
            if (damageCoroutine == null)
                damageCoroutine = StartCoroutine(ApplyDamageOverTime());
        }
        else
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator ApplyDamageOverTime()
    {
        while (true)
        {
            foreach (PlayerObj player in PlayerManager.Instance.playersInGame.Values)
            {
                if (player.inOutpost) continue;
                player.TakeDamage(damageAmount);
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }
}
