using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public ItemDatabase database; // assign in Inspector
    public GameObject itemPrefab; // assign in Inspector
    void Start()
    {
        var spawnPoints = GetComponentsInChildren<SpawnPoint>();
        foreach (var sp in spawnPoints)
        {
            if (Random.value <= sp.spawnChance)
            {
                ItemData[] pool = null;

                switch (sp.itemType)
                {
                    case ItemType.FloorItem:
                        pool = database.floorItems;
                        break;
                    case ItemType.TableItem:
                        pool = database.tableItems;
                        break;
                    case ItemType.ShelfItem:
                        pool = database.shelfItems;
                        break;
                    case ItemType.OutdoorItem:
                        pool = database.outdoorItems;
                        break;
                }

                if (pool != null && pool.Length > 0)
                {
                    int index = Random.Range(0, pool.Length);
                    var prefab = pool[index].prefab; // get the prefab from ItemData

                    if (prefab != null)
                    {
                        var spawnedItem = Instantiate(itemPrefab, sp.transform.position, sp.transform.rotation, transform);
                        spawnedItem.GetComponent<PickUppableItem>().itemData = pool[index];

                    }
                }
            }
        }
    }
}
