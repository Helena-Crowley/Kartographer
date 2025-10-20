using UnityEngine;
using System.Collections.Generic;

public class StormDealDamage : MonoBehaviour
{
    public int damagePerTick = 5;
    public float tickRate = 1f;

    private Dictionary<Collider, float> timers = new Dictionary<Collider, float>();
    private HashSet<Collider> insidePlayers = new HashSet<Collider>();


    private void Update()
    {
        foreach (var col in new List<Collider>(insidePlayers))
        {
            timers[col] += Time.deltaTime;
            var player = col.GetComponent<PlayerStats>();
            if (player == null) return;
            if (player != null && timers[col] >= tickRate)
            {
                player.TakeDamage(damagePerTick);
                timers[col] = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !insidePlayers.Contains(other))
        {
            insidePlayers.Add(other);
            if (!timers.ContainsKey(other))
                timers[other] = 0f;
            Debug.Log("Entered storm: " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && insidePlayers.Contains(other))
        {
            insidePlayers.Remove(other);
            timers.Remove(other);
            Debug.Log("Exited storm: " + other.name);
        }
    }

}
