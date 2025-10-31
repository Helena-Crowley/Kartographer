using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerObj : NetworkBehaviour
{
    [Header("Player Stats")]
    public int health = 100;
    public int stamina = 100;
    public float distanceFromStorm = 0f;
    public float walletAmount = 0f;
    public ItemData[] inventoryItems = new ItemData[5];
    public ulong playerId; //who we are

    public float moveSpeed = 5f;
    public float jumpHeight = 1f;
    public float staminaDrainRate = 5f;
    public int staminaDrainSpeed = 10;

    public InputActionMap currentInputMapping = null;

    [Header("UI References")]
    public HealthBar healthBar;
    public StaminaBar staminaBar;


    //-----Local Variables-----
    [HideInInspector]
    public float currentStamina;
    private float maxStamina;

    [HideInInspector]
    private int currentHealth;
    private int maxHealth;


    [HideInInspector] public bool nearCart = false;
    [HideInInspector] public bool inCart = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        playerId = GetComponent<NetworkObject>().OwnerClientId;
        Debug.Log("Player: "+ playerId + " was spawned in");

        currentHealth = health;
        maxHealth = health;

        currentStamina = stamina;
        maxStamina = stamina;
    }

    /* ---------------------------HEALTH STUFF------------------------*/


    public void ApplyDamage(int dmg)
    {
        if (!IsLocalPlayer) return;

        currentHealth -= dmg;
        Debug.Log("Damage applied = " + dmg + "to " + playerId);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (!IsLocalPlayer) return;
        healthBar.UpdateHealthBar(currentHealth);
    }


    /* ---------------------------STAMINA STUFF------------------------*/
    public void DrainStamina()
    {
        if (!IsLocalPlayer) return;
        Debug.Log("Draining Player: " + playerId + "s stamina");
        currentStamina -= staminaDrainRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }

    public void RegainStamina()
    {
        if (!IsLocalPlayer) return;

        currentStamina += staminaDrainRate / 2 * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        if (!IsLocalPlayer) return;
        staminaBar.UpdateStaminaBar(currentStamina);
    }
}