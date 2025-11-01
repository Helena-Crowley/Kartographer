using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class StormDealDamage : NetworkBehaviour
{
    [Header("Storm Settings")]
    [SerializeField] private int damagePerTick = 2;
    [SerializeField] private float tickRate = 1f;

    // Track which players are inside the storm and their timers
    private List<PlayerObj> insidePlayers = new List<PlayerObj>();
    private Dictionary<PlayerObj, float> timers = new Dictionary<PlayerObj, float>();

    private void Update()
    {
        //if (!IsServer) return;

        foreach (var player in insidePlayers)
        {
            timers[player] += Time.deltaTime;

            if (timers[player] >= tickRate)
            {
                timers[player] = 0f;

                // Use the new TakeDamage method on PlayerObj
                player.TakeDamage(damagePerTick);

                Debug.LogWarning($"Storm applied {damagePerTick} damage to Player {player.playerId}");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (!IsServer) return;
        if (!other.CompareTag("Player")) return;

        PlayerObj playerObj = other.GetComponent<PlayerObj>();
        if (playerObj != null && !insidePlayers.Contains(playerObj))
        {
            AddPlayerToInsidePlayerList(playerObj);
            Debug.LogWarning($"{playerObj.playerId} entered storm");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (!IsServer) return;
        if (!other.CompareTag("Player")) return;

        PlayerObj playerObj = other.GetComponent<PlayerObj>();
        if (playerObj != null && insidePlayers.Contains(playerObj))
        {
            RemovePlayerFromInside(playerObj);
            Debug.Log($"{playerObj.playerId} exited storm");
        }
    }

    private void AddPlayerToInsidePlayerList(PlayerObj playerObj)
    {
        insidePlayers.Add(playerObj);
        timers[playerObj] = 0f; // Initialize timer
    }

    private void RemovePlayerFromInside(PlayerObj playerObj)
    {
        insidePlayers.Remove(playerObj);
        timers.Remove(playerObj);
    }
}
