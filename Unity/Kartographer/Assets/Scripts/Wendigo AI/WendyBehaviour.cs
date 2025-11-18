using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class WendyBehaviour : NetworkBehaviour
{
    private WendySpawner wendySpawner;
    private List<GameObject> players = new List<GameObject>();

    [Range(0, 180f)] public float viewAngle = 135f;
    [SerializeField] private float viewDistance = 150f;
    [SerializeField] private float chaseDistance = 75f;
    public LayerMask obstacleMask;

    private bool spotted = false;
    [SerializeField] private int chanceOfChase = 50; // 0-100, higher = more likely to chase
    [SerializeField] private float despawnTimer = 30f;

    [SerializeField] private NavMeshAgent navAgent;
    private GameObject playerToChase;

    private float despawnCountdown;
    private bool isDespawning = false;
    private bool inRange = false;

    [SerializeField] private AudioClip wendyScreechSound;
    [SerializeField] private WendyAnimator animController;
    [SerializeField] private float minVol = 0.1f;
    [SerializeField] private float maxVol = 0.1f;
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private AudioClip chaseMusic;
    [SerializeField] private float attackDistance;

    public void InitializePlayers(List<GameObject> playerList)
    {
        players = new List<GameObject>(playerList);
        Debug.Log($"WendyBehaviour initialized with {players.Count} players");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            wendySpawner = FindAnyObjectByType<WendySpawner>();

            // Fallback if InitializePlayers wasn't called
            if (players.Count == 0 && wendySpawner != null)
            {
                players = wendySpawner.GetPlayerList();
                Debug.Log($"WendyBehaviour got {players.Count} players from spawner");
            }

            despawnCountdown = despawnTimer;
        }
    }

    void Update()
    {
        if (!IsServer) return;

        if (CheckIfPlayerSeesMe() && !spotted)
        {
            spotted = true;
            despawnCountdown = despawnTimer; // Reset timer
            StartCoroutine(FightOrFlight());
        }
        else if (!spotted)
        {
            despawnCountdown -= Time.deltaTime;
            if (despawnCountdown <= 0 && !isDespawning)
            {
                isDespawning = true;
                DespawnSelf();
            }
        }
    }

    private bool CheckIfPlayerSeesMe()
    {
        if (players == null || players.Count == 0)
        {
            return false;
        }

        foreach (var player in players)
        {
            if (player == null)
            {
                Debug.LogWarning("Null player in list");
                continue;
            }

            Vector3 dirFromPlayerToMe = (transform.position - player.transform.position).normalized;
            float distToPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (distToPlayer > viewDistance)
                continue;

            float angle = Vector3.Angle(player.transform.forward, dirFromPlayerToMe);

            if (angle > viewAngle / 2)
            {
                Debug.Log($"Player {player.name} not looking at Wendy (angle too wide)");
                continue;
            }

            // Line of sight check from player's eye position to Wendy
            Vector3 playerEyePos = player.transform.position + Vector3.up * 1.8f;
            Vector3 wendyCenter = transform.position + Vector3.up * 1f;
            Vector3 dirToCheck = (wendyCenter - playerEyePos).normalized;
            float distToCheck = Vector3.Distance(playerEyePos, wendyCenter);

            Debug.DrawRay(playerEyePos, dirToCheck * distToCheck, Color.red, 0.1f);

            if (Physics.Raycast(playerEyePos, dirToCheck, distToCheck, obstacleMask))
            {
                continue;
            }

            //Debug.Log($"Player {player.name} CAN SEE me!");
            playerToChase = player;
            return true;
        }

        return false;
    }

    IEnumerator FightOrFlight()
    {

        //float decisionTime = Random.Range(3f, 6f);

        //yield return new WaitForSeconds(decisionTime);

        int randValue = Random.Range(0, 100);

        if (randValue >= chanceOfChase)
        {
            DespawnSelf();
        }
        else
        {
            if (playerToChase != null && navAgent != null)
            {
                SoundManager.Instance.PlaySound(viewDistance / 6, 5, wendyScreechSound, transform.position, "SFX", 0.2f);
                animController.PlayChase();

                yield return new WaitForSeconds(2f);
                SoundManager.Instance.PlayMusic(chaseMusic, "SFX", .15f, 2);
                StartChasing();
            }
        }
    }

    IEnumerator ChasePlayer()
    {
        while (playerToChase != null && navAgent != null && navAgent.enabled && inRange)
        {
            Debug.Log("chaseDistance" + chaseDistance);
            if (Vector3.Distance(transform.position, playerToChase.transform.position) > chaseDistance) inRange = false;
            if (Vector3.Distance(transform.position, playerToChase.transform.position) <= attackDistance)
            {
                StartCoroutine(AttackPlayer());
                yield break;
            }
            navAgent.SetDestination(playerToChase.transform.position);
            yield return new WaitForSeconds(0.5f);
        }
        SoundManager.Instance.StopMusic(2);
        yield return new WaitForSeconds(2f);
        wendySpawner.DespawnWendy();

    }

    IEnumerator AttackPlayer()
    {
        navAgent.SetDestination(transform.position);
        //playu hit animatiohn
        animController.StartAttack();
        playerToChase.GetComponent<PlayerObj>().TakeDamage(15);
        yield return new WaitForSeconds(2.267f);//change to animation time
        animController.StopAttack();
        StartChasing();

    }

    private void DespawnSelf()
    {
        if (wendySpawner != null)
        {
            wendySpawner.DespawnWendy();
        }
        else
        {
            Debug.LogWarning("No spawner reference, destroying directly");
            if (GetComponent<NetworkObject>() != null)
            {
                GetComponent<NetworkObject>().Despawn();
            }
            Destroy(gameObject);
        }
    }

    public void PlayWendyFootstep()
    {
        // Pick random sound and volume
        int i = Random.Range(0, footsteps.Length);
        AudioClip randomSound = footsteps[i];
        float volume = Random.Range(minVol, maxVol);

        SoundManager.Instance.PlaySound(10, 60, randomSound, transform.position, "SFX", volume, true);
    }

    private void StartChasing()
    {
        navAgent.enabled = true;
        inRange = true;
        StartCoroutine(ChasePlayer());
    }
}