using Unity.Netcode;
using UnityEngine;

public class PickUppableItem : NetworkBehaviour
{
    [HideInInspector] public ItemData itemData;

    [SerializeField] private NetworkVariable<int> itemId = new NetworkVariable<int>(-1);

    private ItemDatabase database;

    public void Initialize(ItemData data, int id, ItemDatabase db)
    {
        itemData = data;
        database = db;

        if (IsServer)
            itemId.Value = id; // safe: only server writes NetworkVariable
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Only clients need this
        if (!IsServer)
        {
            itemId.OnValueChanged += OnItemIdChanged;

            // If already set, apply immediately
            if (itemId.Value >= 0)
                SetupItemData(itemId.Value);
        }
    }


    private void Start()
    {
        // On the server, we already have itemData from Initialize()
        if (IsServer)
            CreateMesh();
    }

    private void CreateMesh()
    {
        if (itemData == null || itemData.prefab == null) return;

        GameObject meshInstance = Instantiate(itemData.prefab, transform.position, itemData.prefab.transform.rotation, transform);
        meshInstance.transform.localScale = itemData.defaultScale;

        Renderer rend = meshInstance.GetComponent<Renderer>();
        if (rend != null)
            meshInstance.transform.localPosition = new Vector3(0, rend.bounds.extents.y, 0);
        else
            meshInstance.transform.localPosition = Vector3.zero;

        MeshCollider meshCol = meshInstance.GetComponent<MeshCollider>();
        if (meshCol == null)
            meshCol = meshInstance.AddComponent<MeshCollider>();

        meshCol.sharedMesh = meshInstance.GetComponent<MeshFilter>().sharedMesh;
        meshCol.convex = true;
    }

    public void OnPickup(GameObject player)
    {
        Debug.Log($"{player.name} picked up {itemData.displayName}");
        itemData.owner = player;

        Inventory inventory = player.GetComponent<Inventory>();
        InventoryIconGenerator iconGenerator = player.GetComponent<InventoryIconGenerator>();

        if (inventory != null)
        {
            inventory.Add(itemData);

            if (iconGenerator != null)
            {
                int slotIndex = iconGenerator.GetNextAvailableSlot();
                if (slotIndex >= 0)
                {
                    iconGenerator.GenerateIcon(itemData, slotIndex);
                }
                else
                {
                    Debug.LogWarning("No available inventory slots!");
                }
            }
        }
        else
        {
            Debug.LogWarning("Player has no Inventory component!");
        }

        if (IsServer)
        {
            NetworkObject netObj = GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Despawn(); // syncs destruction across the network
            }

            Destroy(gameObject, 0.1f);
        }
        else
        {
            ulong playerId = player.GetComponent<NetworkObject>().OwnerClientId;
            RequestPickupServerRpc(playerId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPickupServerRpc(ulong playerId)
    {
        GameObject player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerId).gameObject;
        OnPickup(player); // call the same logic on the server
    }

    private void OnItemIdChanged(int oldValue, int newValue)
    {
        SetupItemData(newValue);
    }

    private void SetupItemData(int id)
    {
        if (database == null)
            database = FindFirstObjectByType<ItemDatabase>();

        if (database != null && id >= 0)
        {
            itemData = database.GetItemById(id);
            CreateMesh();
        }
        else
        {
            Debug.LogWarning($"{name} failed to load itemData on client (id={id})");
        }
    }


}

