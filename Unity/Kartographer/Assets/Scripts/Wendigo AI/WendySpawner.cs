using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WendySpawner : MonoBehaviour
{
    private List<GameObject> players = new List<GameObject>();
    [HideInInspector] public bool canSpawn = false;
    private bool isSpawning = false;

    [SerializeField] private int spawnDelayMin = 10;
    [SerializeField] private int spawnDelayMax = 15;

    [SerializeField] private float spawnDistanceFromPlayer;
    [SerializeField] private GameObject wendyPrefab;

    private GameObject wendy;

    public void AddPlayer(GameObject player)
    {
        var playerObj = player.GetComponent<PlayerObj>();
        if (!players.Contains(player))
        {
            players.Add(player);
        }

        if (playerObj != null)
            playerObj.OnInCartChanged += HandleInCartChanged;
    }

    private void HandleInCartChanged(bool inCart)
    {
        if (!inCart)
        {
            canSpawn = true;
            if (!isSpawning && wendy == null)
            {
                StartCoroutine(SpawnWithDelay());
            }
        }
        else
        {
            canSpawn = false;
        }
    }

    private void OnDestroy()
    {
        foreach (var player in players)
        {
            if (player != null)
                player.GetComponent<PlayerObj>().OnInCartChanged -= HandleInCartChanged;
        }
    }

    IEnumerator SpawnWithDelay()
    {
        isSpawning = true;
        int spawnDelay = Random.Range(spawnDelayMin, spawnDelayMax);

        Debug.Log($"Wendy will spawn in {spawnDelay} seconds");
        yield return new WaitForSeconds(spawnDelay);

        if (canSpawn && wendy == null)
        {
            SpawnWendy();
        }

        isSpawning = false;
    }

    void SpawnWendy()
    {
        if (players.Count == 0)
        {
            isSpawning = false;
            return;
        }

        int index = Random.Range(0, players.Count);
        GameObject chosenOne = players[index];

        Vector3 spawnPosition = chosenOne.transform.position - chosenOne.transform.forward * spawnDistanceFromPlayer;

        if (Physics.Raycast(spawnPosition + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 75f))
        {
            spawnPosition = hit.point;
        }

        Quaternion spawnRotation = Quaternion.LookRotation(chosenOne.transform.position - spawnPosition);
        wendy = Instantiate(wendyPrefab, spawnPosition, spawnRotation);

        var wendyBehaviour = wendy.GetComponent<WendyBehaviour>();
        if (wendyBehaviour != null)
        {
            wendyBehaviour.InitializePlayers(players);
        }

        wendy.GetComponent<NetworkObject>().Spawn(true);

        Debug.Log("Wendy has been spawned");
    }

    public void DespawnWendy()
    {
        if (wendy != null)
        {
            wendy.GetComponent<WendyBehaviour>().playerToChase = null;
            // Play disappear effect/sound here
            if (wendy.GetComponent<NetworkObject>() != null)
            {
                wendy.GetComponent<NetworkObject>().Despawn();
            }
            Destroy(wendy);
            wendy = null;
        }

        if (canSpawn && !isSpawning)
        {
            StartCoroutine(SpawnWithDelay());
        }
    }

    public List<GameObject> GetPlayerList()
    {
        return players;
    }
}