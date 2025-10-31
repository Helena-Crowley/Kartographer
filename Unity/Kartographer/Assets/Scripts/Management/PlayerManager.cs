using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private Dictionary<ulong, PlayerObj> playersInGame = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterPlayer(PlayerObj player)
    {
        if (!IsServer) return; // Only the server tracks the list
        playersInGame[player.OwnerClientId] = player;
    }

    public void UnregisterPlayer(PlayerObj player)
    {
        if (!IsServer) return;
        playersInGame.Remove(player.OwnerClientId);
    }

    // Letting everyone (including you) know you took damage (Step2)
    // public void TakeDamage(ulong clientId, int damage)
    // {
    //     if (!IsServer) return;

    //     if (playersInGame.TryGetValue(clientId, out PlayerObj player))
    //     {
    //         player.ApplyDamage(damage);
    //     }
    // }
}
