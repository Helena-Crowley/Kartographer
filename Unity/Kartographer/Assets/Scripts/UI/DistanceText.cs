// using UnityEngine;
// using TMPro;
// using Unity.Netcode;
// using System.Collections;

// public class DistanceText : NetworkBehaviour
// {
//     [Header("UI Elements")]
//     public TMP_Text distanceText;

//     [Header("World References")]
//     public Transform stormTransform;

//     private Transform playerTransform;
//     private float updateInterval = 0.5f;

//     public override void OnNetworkSpawn()
//     {
//         if (!IsOwner) return; // Only local player updates their own UI
//         PlayerUIManager.Instance.BindPlayer(this);

//         playerTransform = transform; // Assumes this script is on the player prefab

//         // Initialize text
//         if (distanceText != null)
//             distanceText.text = StormDistance().ToString("F1") + "m";

//         // Start updating distance periodically
//         StartCoroutine(UpdateDistanceCoroutine());
//     }

//     private IEnumerator UpdateDistanceCoroutine()
//     {
//         while (true)
//         {
//             if (distanceText != null)
//                 distanceText.text = StormDistance().ToString("F1") + "m";

//             yield return new WaitForSeconds(updateInterval);
//         }
//     }

//     private float StormDistance()
//     {
//         if (playerTransform == null || stormTransform == null) return 0f;

//         // Use full 3D distance
//         return playerTransform.position.x - stormTransform.position.x;
//     }
// }
