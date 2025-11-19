using UnityEngine;
using Unity.Netcode;

public class CartSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject cartPrefab;
    [SerializeField] private Vector3 spawnPosition = new Vector3(0, 0, 0);
    [SerializeField] private Quaternion spawnRotation = Quaternion.identity;

    [HideInInspector] public CartInteraction cartInteraction;

    private GameObject currentCart;

    public override void OnNetworkSpawn()
    {
        SpawnCart();
        // if (IsServer)
        // {
        //     // Spawn the cart once on the server
        //     var cart = Instantiate(cartPrefab, spawnPosition, spawnRotation);
        //     currentCart = cart;
        //     cart.GetComponent<NetworkObject>().Spawn(true);
        //     //Debug.Log("Cart spawned on server!");
        // }
    }

    public void ResetCart()
    {
        RemoveCart();
        SpawnCart();
    }

    private void SpawnCart()
    {
        if (!IsServer) return;

        var cart = Instantiate(cartPrefab, spawnPosition, spawnRotation);
        currentCart = cart;
        cartInteraction = currentCart.GetComponent<CartInteraction>();
        cart.GetComponent<NetworkObject>().Spawn(true);
    }

    private void RemoveCart()
    {
        if (!IsServer) return;
        Destroy(currentCart);
    }
}
