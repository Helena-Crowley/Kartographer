// using UnityEngine;
// using Unity.Netcode;
// using UnityEngine.InputSystem;

// public class PlayerObj : NetworkBehaviour
// {
//     [Header("Player Stats")]
//     public int health = 100;
//     public int stamina = 100;
//     public float distanceFromStorm = 0f;
//     public float walletAmount = 0f;
//     public ItemData[] inventoryItems = new ItemData[5];
//     public ulong playerId;

//     public float moveSpeed = 5f;
//     public float jumpHeight = 1f;
//     public float staminaDrainRate = 5f;
//     public int staminaDrainSpeed = 10;

//     [Header("UI References")]
//     public HealthBar healthBar;
//     public StaminaBar staminaBar;

//     //================ Networked Variables ==================
//     public NetworkVariable<int> currentHealth = new(
//         100,
//         NetworkVariableReadPermission.Everyone,       // everyone can read
//         NetworkVariableWritePermission.Server         // only server can write
//     );

//     //================ Local Variables ==================
//     [HideInInspector] public float currentStamina;
//     private float maxStamina;
//     private int maxHealth;

//     [HideInInspector] public bool nearCart = false;
//     [HideInInspector] public bool inCart = false;

//     //======================================================
//     public override void OnNetworkSpawn()
//     {
//         playerId = OwnerClientId;
//         maxHealth = health;
//         maxStamina = stamina;
//         currentStamina = stamina;

//         currentHealth.OnValueChanged += OnHealthChanged;

//         if (IsOwner)
//         {
//             Debug.Log($"Binding UI for local player {OwnerClientId}");
//             PlayerUIManager.Instance.BindPlayer(this);

//             // Force initial update after binding
//             if (healthBar != null)
//                 healthBar.UpdateHealthBar(currentHealth.Value);
//             if (staminaBar != null)
//                 staminaBar.UpdateStaminaBar(currentStamina);
//         }
//     }

//     private void OnHealthChanged(int oldVal, int newVal)
//     {
//         Debug.Log($"[{(IsOwner ? "LOCAL" : "REMOTE")}] Player {OwnerClientId}: Health {oldVal} → {newVal}");

//         // Update health UI for everyone, not just owner
//         if (healthBar != null)
//             healthBar.UpdateHealthBar(newVal);
//     }

//     private void OnDestroy()
//     {
//         currentHealth.OnValueChanged -= OnHealthChanged;
//     }

//     //======================================================
//     /* ------------------ HEALTH ------------------ */

//     /// <summary>
//     /// Call this from any script to apply damage.
//     /// Handles server/client logic automatically.
//     /// </summary>
//     public void TakeDamage(int dmg)
//     {
//         if (IsServer)
//         {
//             currentHealth.Value = Mathf.Clamp(currentHealth.Value - dmg, 0, maxHealth);
//             Debug.Log($"Server applied {dmg} damage to Player {OwnerClientId}");
//         }
//         else
//         {
//             TakeDamageServerRpc(dmg);
//         }
//     }

//     [ServerRpc(RequireOwnership = false)]
//     private void TakeDamageServerRpc(int dmg)
//     {
//         currentHealth.Value = Mathf.Clamp(currentHealth.Value - dmg, 0, maxHealth);
//         Debug.Log($"Server applied {dmg} damage via RPC to Player {OwnerClientId}");
//     }

//     //======================================================
//     /* ------------------ STAMINA ------------------ */

//     public void DrainStamina()
//     {
//         if (!IsOwner) return;

//         currentStamina -= staminaDrainRate * Time.deltaTime;
//         currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
//         staminaBar.UpdateStaminaBar(currentStamina);
//     }

//     public void RegainStamina()
//     {
//         if (!IsOwner) return;

//         currentStamina += (staminaDrainRate / 2) * Time.deltaTime;
//         currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
//         staminaBar.UpdateStaminaBar(currentStamina);
//     }
// }
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerObj : NetworkBehaviour
{
    // ================= PLAYER STATS =================
    [Header("Player Stats")]
    public int health = 100;
    public int stamina = 100;
    public float distanceFromStorm = 0f;
    public float walletAmount = 0f;
    public ItemData[] inventoryItems = new ItemData[5];
    public ulong playerId;
    public bool isAlive = true;//might need to be a networked variable

    // Movement & stat tuning
    public float moveSpeed = 5f;
    public float jumpHeight = 1f;
    public float staminaDrainRate = 5f;
    public int staminaDrainSpeed = 10;

    [Header("UI References")]
    public HealthBar healthBar;
    public StaminaBar staminaBar;

    [SerializeField] private AudioClip impactSFX;

    // ================= NETWORKED VARIABLES =================
    // NetworkVariables automatically sync between server and all clients.
    // - Default value: 100
    // - Readable by everyone
    // - Writable only by the server
    public NetworkVariable<int> currentHealth = new(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // ================= LOCAL VARIABLES =================
    [HideInInspector] public float currentStamina;
    private float maxStamina;
    private int maxHealth;

    [HideInInspector] public bool nearCart = false;

    // inCart event
    public event System.Action<bool> OnInCartChanged;
    private bool _inCart;
    public bool InCart
    {
        get => _inCart;
        set
        {
            if (_inCart == value) return; // no change
            _inCart = value;
            OnInCartChanged?.Invoke(_inCart);
        }
    }

    // ================= LIFECYCLE =================
    public override void OnNetworkSpawn()
    {
        // This runs automatically when the object is spawned on the network
        // (either by the server or as part of a network scene).
        // All clients that have this object will execute this method.

        playerId = OwnerClientId;  // Each networked object knows who owns it.
        maxHealth = health;
        maxStamina = stamina;
        currentStamina = stamina;
        isAlive = true;

        // Listen for changes in the NetworkVariable (synced across all clients)
        currentHealth.OnValueChanged += OnHealthChanged;

        // If this script is running on the local player's instance
        // (the client that owns this PlayerObj), then bind its UI elements.
        if (IsOwner)
        {
            //Debug.Log($"Binding UI for local player {OwnerClientId}");
            PlayerUIManager.Instance.BindPlayer(this);

            // Initialize UI with current values
            if (healthBar != null)
                healthBar.UpdateHealthBar(currentHealth.Value);
            if (staminaBar != null)
                staminaBar.UpdateStaminaBar(currentStamina);
        }
    }

    // Called automatically when the health NetworkVariable changes
    // on *any* client (local or remote).
    private void OnHealthChanged(int oldVal, int newVal)
    {
        //Debug.Log($"[{(IsOwner ? "LOCAL" : "REMOTE")}] Player {OwnerClientId}: Health {oldVal} → {newVal}");

        // Update health bar on all clients (each instance has its own UI ref)
        if (healthBar != null)
            healthBar.UpdateHealthBar(newVal);
    }

    public override void OnDestroy()
    {
        // Always unsubscribe from events to avoid leaks when object despawns
        currentHealth.OnValueChanged -= OnHealthChanged;

        base.OnDestroy();
    }

    // ======================================================
    /* ------------------ HEALTH SYSTEM ------------------ */

    /// <summary>
    /// Call this method from *any* script or object to apply damage.
    /// Handles both client → server and server-side logic automatically.
    /// </summary>
    public void TakeDamage(int dmg)
    {
        //dmg sfx
        SoundManager.Instance.PlaySound2D(impactSFX, "SFX", 0.15f, true);
        //screen red
        // If we're the server, we can modify the NetworkVariable directly.
        if (IsServer)
        {
            currentHealth.Value = Mathf.Clamp(currentHealth.Value - dmg, 0, maxHealth);
            //Debug.Log($"Server applied {dmg} damage to Player {OwnerClientId}");
        }
        else
        {
            // If we're a client, we can't write to currentHealth directly.
            // Instead, we send an RPC to the server to perform the change.
            TakeDamageServerRpc(dmg);
        }

        if (isAlive && currentHealth.Value <= 0)
        {
            isAlive = false;
            KillPlayer();
        }
    }

    // This ServerRpc is executed *on the server* when called from any client.
    // [RequireOwnership = false] allows other clients (not just the owner)
    // to damage this player — useful for PvP, hazards, etc.
    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(int dmg)
    {
        // The server safely updates the authoritative NetworkVariable.
        currentHealth.Value = Mathf.Clamp(currentHealth.Value - dmg, 0, maxHealth);
        //Debug.Log($"Server applied {dmg} damage via RPC to Player {OwnerClientId}");
    }

    // ======================================================
    /* ------------------ STAMINA SYSTEM ------------------ */

    public void DrainStamina()
    {
        // Stamina is local-only here (not a NetworkVariable),
        // so only update it for the player that owns this instance.
        if (!IsOwner) return;

        currentStamina -= staminaDrainRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaBar.UpdateStaminaBar(currentStamina);
    }

    public void RegainStamina()
    {
        if (!IsOwner) return;

        currentStamina += (staminaDrainRate / 2) * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaBar.UpdateStaminaBar(currentStamina);
    }

    void KillPlayer()
    {
        //death sfx
        //death screen
        Debug.Log("player died");
        int index = Random.Range(0, GameManager.Instance.outpostSpawnPoints.Length);
        Vector3 newPosition = GameManager.Instance.outpostSpawnPoints[index].position;
        GetComponent<NetworkObject>().transform.position = newPosition;
    }
}

//set up gamemanager spawn pts 