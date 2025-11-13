using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class WendySpawner : MonoBehaviour
{
    private List<GameObject> players = new List<GameObject>();
    //wait until players have been in cart before can spawn
    private bool canSpawn = false;

    [SerializeField] private int spawnDelayMin = 10;
    [SerializeField] private int spawnDelayMax = 15;
    private int spawnDelay;

    [SerializeField] private float spawnDistanceFromPlayer;
    [SerializeField] private GameObject wendyPrefab;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private GameObject wendy;

    public void AddPlayer(GameObject player)
    {
        var playerObj = player.GetComponent<PlayerObj>();
        players.Add(player);
        if (playerObj != null)
            playerObj.OnInCartChanged += HandleInCartChanged;
    }

    // gets called if the bool "inCart" is changed
    private void HandleInCartChanged(bool inCart)
    {
        if (!inCart)
            canSpawn = true;
        else
            canSpawn = false;
    }

    void Update()
    {
        if (canSpawn)
        {
            StartCoroutine(SpawnWithDelay());
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
        Debug.Log("trying to spawn wendy");
        spawnDelay = Random.Range(spawnDelayMin, spawnDelayMax);
        canSpawn = false;
        yield return new WaitForSeconds(spawnDelay);
        SpawnWendy();
    }

    void SpawnWendy()
    {
        var index = Random.Range(0, players.Count-1);
        var chosenOne = players[index];

        //change this to have a raycast down (dont spawn underground)
        spawnPosition = chosenOne.transform.position;
        spawnPosition.z += spawnDistanceFromPlayer;
        spawnRotation = chosenOne.transform.rotation;
        wendy = Instantiate(wendyPrefab, spawnPosition, spawnRotation);
        wendy.GetComponent<NetworkObject>().Spawn(true);

        wendy.GetComponent<WendyBehaviour>().GetPlayers(players);
        Debug.Log("wendy has been spawned");
    }
    
    public void DespawnWendy()
    {
        //play dissapear noise
        Destroy(wendy);
        canSpawn = true;
    }
}
