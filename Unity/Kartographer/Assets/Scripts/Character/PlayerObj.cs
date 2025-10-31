using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerObj : NetworkBehaviour
{
    [Header("Player Stats")]
    public int health = 100;
    public int stamina = 100;
    public float distanceFromStorm = 0f;
    public float walletAmount = 0f;
    public ItemData[] inventoryItems = new ItemData[5];
    public ulong playerId;

    public float moveSpeed = 5f;
    public float jumpHeight = 1f;
    public float staminaDrainRate = 5f;
    public int staminaDrainSpeed = 10;

    [Header("UI References")]
    public HealthBar healthBar;
    public StaminaBar staminaBar;

    //================ Networked Variables ==================
    public NetworkVariable<int> currentHealth = new(
        100,
        NetworkVariableReadPermission.Everyone,       // everyone can read
        NetworkVariableWritePermission.Server         // only server can write
    );

    //================ Local Variables ==================
    [HideInInspector] public float currentStamina;
    private float maxStamina;
    private int maxHealth;

    [HideInInspector] public bool nearCart = false;
    [HideInInspector] public bool inCart = false;

    //======================================================
    public override void OnNetworkSpawn()
    {
        playerId = OwnerClientId;
        maxHealth = health;
        maxStamina = stamina;
        currentStamina = stamina;

        currentHealth.OnValueChanged += OnHealthChanged;

        if (IsOwner)
        {
            Debug.Log($"Binding UI for local player {OwnerClientId}");
            PlayerUIManager.Instance.BindPlayer(this);

            // Force initial update after binding
            if (healthBar != null)
                healthBar.UpdateHealthBar(currentHealth.Value);
            if (staminaBar != null)
                staminaBar.UpdateStaminaBar(currentStamina);
        }
    }

    private void OnHealthChanged(int oldVal, int newVal)
    {
        Debug.Log($"[{(IsOwner ? "LOCAL" : "REMOTE")}] Player {OwnerClientId}: Health {oldVal} → {newVal}");

        // Update health UI for everyone, not just owner
        if (healthBar != null)
            healthBar.UpdateHealthBar(newVal);
    }

    private void OnDestroy()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    //======================================================
    /* ------------------ HEALTH ------------------ */

    /// <summary>
    /// Call this from any script to apply damage.
    /// Handles server/client logic automatically.
    /// </summary>
    public void TakeDamage(int dmg)
    {
        if (IsServer)
        {
            currentHealth.Value = Mathf.Clamp(currentHealth.Value - dmg, 0, maxHealth);
            Debug.Log($"Server applied {dmg} damage to Player {OwnerClientId}");
        }
        else
        {
            TakeDamageServerRpc(dmg);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(int dmg)
    {
        currentHealth.Value = Mathf.Clamp(currentHealth.Value - dmg, 0, maxHealth);
        Debug.Log($"Server applied {dmg} damage via RPC to Player {OwnerClientId}");
    }

    //======================================================
    /* ------------------ STAMINA ------------------ */

    public void DrainStamina()
    {
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
}
