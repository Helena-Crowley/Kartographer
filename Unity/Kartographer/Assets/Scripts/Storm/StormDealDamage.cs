using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class StormDealDamage : NetworkBehaviour
{
    [Header("Storm Settings")]
    [SerializeField] private int damagePerTick = 2;
    [SerializeField] private float tickRate = 1f;

    // Track which players are inside the storm and their timers
    private HashSet<ulong> insidePlayers = new HashSet<ulong>();
    private Dictionary<ulong, float> timers = new Dictionary<ulong, float>();

    private void Update()
    {
        if (!IsServer) return; // Only server handles damage

        foreach (var playerId in new List<ulong>(insidePlayers))
        {
            // Safely initialize timer if missing
            if (!timers.ContainsKey(playerId))
                timers[playerId] = 0f;

            timers[playerId] += Time.deltaTime;

            if (timers[playerId] >= tickRate)
            {
                timers[playerId] = 0f;

                if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out var netObj) && netObj != null)
                {
                    // Get PlayerStats from root or children
                    var playerStats = netObj.GetComponent<PlayerStats>() 
                                      ?? netObj.GetComponentInChildren<PlayerStats>();

                    if (playerStats != null)
                    {
                        playerStats.TakeDamageServerRpc(damagePerTick);
                        Debug.Log($"[Server] Dealt {damagePerTick} damage to {netObj.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"[Server] PlayerStats not found on {netObj.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[Server] No spawned object found for ID {playerId}");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var netObj = other.GetComponentInParent<NetworkObject>();
        if (netObj != null && !insidePlayers.Contains(netObj.NetworkObjectId))
        {
            // Let the server handle adding player and initializing timer
            AddPlayerServerRpc(netObj.NetworkObjectId);
            Debug.Log($"[Server] {netObj.name} entered storm");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var netObj = other.GetComponentInParent<NetworkObject>();
        if (netObj != null && insidePlayers.Contains(netObj.NetworkObjectId))
        {
            RemovePlayerServerRpc(netObj.NetworkObjectId);
            Debug.Log($"[Server] {netObj.name} exited storm");
        }
    }

    // ServerRpc to add player to storm
    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerServerRpc(ulong playerId)
    {
        if (!insidePlayers.Contains(playerId))
            insidePlayers.Add(playerId);

        if (!timers.ContainsKey(playerId))
            timers[playerId] = 0f; // Initialize timer
    }

    // ServerRpc to remove player from storm
    [ServerRpc(RequireOwnership = false)]
    private void RemovePlayerServerRpc(ulong playerId)
    {
        insidePlayers.Remove(playerId);
        timers.Remove(playerId);
    }
}
