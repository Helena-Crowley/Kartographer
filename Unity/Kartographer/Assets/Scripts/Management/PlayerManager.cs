using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public class PlayerData
    {
        public PlayerObj playerObj; //currentHealth, currentStamina, isAlive, inOutpost, walletAmount, iconGenerator, playerInventory
    }

    public Dictionary<ulong, PlayerObj> playersInGame = new();
    [SerializeField] private ItemSpawner itemSpawner;
    [SerializeField] private WendySpawner wendySpawner;
    [SerializeField] private CartSpawner cartSpawner;
    [SerializeField] private DayNightCycle dayNightCycle;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterPlayer(NetworkObject player)
    {
        if (!IsServer) return; // Only the server tracks the list
        playersInGame[player.OwnerClientId] = player.GetComponent<PlayerObj>();
        Debug.Log("player registered");
    }

    public void UnregisterPlayer(PlayerObj player)
    {
        if (!IsServer) return;
        playersInGame.Remove(player.OwnerClientId);
    }

    public void ResetGame() // call when ALL players have died to reset
    {
        cartSpawner.cartInteraction.KickEveryoneOut();

        dayNightCycle.ResetDayNightCycle();
        foreach (ItemSpawner itemSpawner in GameManager.Instance.itemSpawners)
        {
            itemSpawner.ResetItems();
        }

        foreach (var kvp in playersInGame)
        {
            PlayerObj player = kvp.Value;
            if (player != null)
            {
                player.ResetPlayer();
                player.isAlive = true;
            }
        }

        wendySpawner.canSpawn = false;
        wendySpawner.DespawnWendy();

        cartSpawner.ResetCart();

    }

    public bool CheckAllPlayersStatus()
    {
        bool allDead = true;

        foreach (var kvp in playersInGame)
        {
            PlayerObj player = kvp.Value;
            if (player != null && player.isAlive)
            {
                allDead = false; // At least one player is alive
                break;
            }
        }

        if (allDead)
        {
            Debug.Log("All players dead. Resetting game...");
            ResetGame();
            return true;
        }
        else
        {
            Debug.Log("Some players are still alive. No reset.");
            return false;
        }
    }

}
