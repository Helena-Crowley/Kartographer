using UnityEngine;
using TMPro;
using Unity.Netcode;

public class DistanceText : NetworkBehaviour
{
    public TMP_Text distanceText;
    public Transform stormTransform;
    private Transform playerTransform;

    private float timer = 0f;
    private float updateInterval = 0.5f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; // only local player shows their own distance

        playerTransform = transform; // assume this script is on the player prefab
        distanceText.text = StormDistance().ToString("F1") + "m";
    }

    void Update()
    {
        if (!IsOwner) return; // only update for local player

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            distanceText.text = StormDistance().ToString("F1") + "m";
            timer = 0f;
        }
    }

    private float StormDistance()
    {
        if (playerTransform == null || stormTransform == null) return 0f;
        return playerTransform.position.x - stormTransform.position.x;
    }
}
