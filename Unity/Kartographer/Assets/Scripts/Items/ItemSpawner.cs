using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    public ItemDatabase database;
    public GameObject itemPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            StartCoroutine(DelayedSpawn());
        }
    }

    private System.Collections.IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnItems();
    }

    private void SpawnItems()
    {
       // Debug.Log($"[ItemSpawner] SpawnItems called. IsServer: {IsServer}");

        var spawnPoints = GetComponentsInChildren<SpawnPoint>();
        //Debug.Log($"[ItemSpawner] Found {spawnPoints.Length} spawn points");

        foreach (var sp in spawnPoints)
        {
            //Debug.Log($"[ItemSpawner] Processing spawn point: {sp.name}, chance: {sp.spawnChance}");

            if (Random.value <= sp.spawnChance)
            {
                ItemData[] pool = null;

                switch (sp.itemType)
                {
                    case ItemType.FloorItem: pool = database.floorItems; break;
                    case ItemType.TableItem: pool = database.tableItems; break;
                    case ItemType.ShelfItem: pool = database.shelfItems; break;
                    case ItemType.OutdoorItem: pool = database.outdoorItems; break;
                }

                if (pool != null && pool.Length > 0)
                {
                    int index = Random.Range(0, pool.Length);
                    var prefab = pool[index].prefab;

                    if (prefab != null)
                    {
                        var spawnedItem = Instantiate(itemPrefab, sp.transform.position, sp.transform.rotation);
                        var netObj = spawnedItem.GetComponent<NetworkObject>();
                        var pickup = spawnedItem.GetComponent<PickUppableItem>();

                        if (netObj != null)
                        {
                            netObj.Spawn(); // Spawn first

                            if (pickup != null)
                            {
                                int itemId = database.GetItemIndex(pool[index]);
                                pickup.Initialize(pool[index], itemId, database); // Now safe to assign NetworkVariable
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"{spawnedItem.name} missing NetworkObject!");
                        }

                    }
                }
            }
        }
    }
}
