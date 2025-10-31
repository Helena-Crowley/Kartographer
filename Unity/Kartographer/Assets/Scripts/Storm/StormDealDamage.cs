using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;

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
        foreach (var player in insidePlayers)
        {
            timers[player] += Time.deltaTime;
            if (timers[player] >= tickRate)
            {
                timers[player] = 0f;
                player.ApplyDamage(damagePerTick);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerObj playerObj = other.GetComponent<PlayerObj>();
        if (playerObj != null && !insidePlayers.Contains(playerObj))
        {
            AddPlayerToInsidePlayerList(playerObj);
            Debug.Log($"{playerObj.playerId} entered storm");
            Debug.LogAssertion(insidePlayers);
        }
        else
        {
            Debug.Log("uh oh thats not right!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerObj playerObj = other.GetComponent<PlayerObj>();
        if (playerObj != null && insidePlayers.Contains(playerObj))
        {
            RemovePlayerFromInside(playerObj);
        }
    }

    private void AddPlayerToInsidePlayerList(PlayerObj playerObj)
    {
        if (!insidePlayers.Contains(playerObj))
            insidePlayers.Add(playerObj);

        if (!timers.ContainsKey(playerObj))
            timers[playerObj] = 0f; // Initialize timer
    }

    private void RemovePlayerFromInside(PlayerObj playerObj)
    {
        insidePlayers.Remove(playerObj);
        timers.Remove(playerObj);
    }
}
