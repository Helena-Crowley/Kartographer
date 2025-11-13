using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class WendyBehaviour : MonoBehaviour
{
    private WendySpawner wendySpawner;
    private List<GameObject> players = new List<GameObject>();
    [Range(0, 180f)] public float viewAngle = 70f;
    public LayerMask obstacleMask;
    private bool spotted = false;
    [SerializeField] private int chanceOfChase;

    void Update()
    {
        if (CheckIfSeen() && !spotted)
        {
            spotted = true;
            //stop despawn timer
            //wait a little before decide to run or disappear call only once
            StartCoroutine(FightOrFlight());

        }
        else
        {
            //start despawn timer
        }
    }

    private bool CheckIfSeen()
    {
        if (players.Count == 0) return false;
        foreach (var player in players)
        {
            Vector3 dirToEnemy = (transform.position - player.transform.position).normalized;
            float distToEnemy = Vector3.Distance(player.transform.position, transform.position);

            // within player's FOV cone
            float angle = Vector3.Angle(player.transform.forward, dirToEnemy);
            if (angle > viewAngle / 2)
                return false;

            // Line of sight check
            if (Physics.Raycast(player.transform.position + Vector3.up * 1.8f, dirToEnemy, distToEnemy, obstacleMask))
                return false;
        }

            return true;
        
    }

    public void GetPlayers(List<GameObject> playerList)
    {
        foreach (var player in playerList)
        {
            players.Add(player);
            Debug.Log("added player " + player);
        }
    }

    IEnumerator FightOrFlight()
    {
        float decisionTime = Random.Range(3, 6);
        yield return new WaitForSeconds(decisionTime);
        int randValue = Random.Range(0, 100);
        if (randValue >= chanceOfChase)
            wendySpawner.DespawnWendy();
        else
        {
            //run at you
            
        }
    }
}
