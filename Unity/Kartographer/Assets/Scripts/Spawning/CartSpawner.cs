using UnityEngine;
using Unity.Netcode;

public class CartSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject cartPrefab;
    [SerializeField] private Vector3 spawnPosition = new Vector3(0, 0, 0);
    [SerializeField] private Quaternion spawnRotation = Quaternion.identity;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Spawn the cart once on the server
            var cart = Instantiate(cartPrefab, spawnPosition, spawnRotation);
            cart.GetComponent<NetworkObject>().Spawn(true);
            //Debug.Log("Cart spawned on server!");
        }
    }
}
