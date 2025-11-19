using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PlayerObj : NetworkBehaviour
{
    // ================= PLAYER STATS =================
    [Header("Player Stats")]
    [SerializeField] private int health = 100;
    [SerializeField] private int stamina = 100;


    //public ItemData[] inventoryItems = new ItemData[5];
    public ulong playerId;

    public DamageVignette dmgScreen;

    // Movement & stat tuning
    public float moveSpeed = 5f;
    public float jumpHeight = 1f;
    public float staminaDrainRate = 5f;
    public int staminaDrainSpeed = 10;
    public Camera playerCamera;

    [Header("UI References")]
    public HealthBar healthBar;
    public StaminaBar staminaBar;

    [SerializeField] private AudioClip impactSFX;


    [SerializeField] private TerminalTyper terminalTyper;
    public TMP_Text outputText;
    public Image deathBGImage;
    private string[] deathScript = new string[]
    {
        "// === System Shutdown ===",
        "[FAIL] Player was too bad...",
        "![ERROR] Connection failed!",
        ">>> YOU DIED <<<"
    };


    public NetworkVariable<int> currentHealth = new(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    [HideInInspector] public float currentStamina;
    public int walletAmount = 0;
    public bool isAlive = true;//might need to be a networked variable
    public bool inOutpost = true;
    public Inventory playerInventory;
    public InventoryIconGenerator iconGenerator;

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

    public override void OnNetworkSpawn()
    {

        playerId = OwnerClientId;
        maxHealth = health;
        maxStamina = stamina;
        currentStamina = stamina;
        isAlive = true;

        currentHealth.OnValueChanged += OnHealthChanged;

        if (IsOwner)
        {
            PlayerUIManager.Instance.BindPlayer(this);

            if (healthBar != null)
                healthBar.UpdateHealthBar(currentHealth.Value);
            if (staminaBar != null)
                staminaBar.UpdateStaminaBar(currentStamina);
        }
    }

    private void OnHealthChanged(int oldVal, int newVal)
    {
        if (healthBar != null)
            healthBar.UpdateHealthBar(newVal);
    }

    public override void OnDestroy()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;

        base.OnDestroy();
    }

    public void TakeDamage(int dmg)
    {
        dmgScreen.StartCoroutine(dmgScreen.ShowDMG());
        //dmg sfx
        SoundManager.Instance.PlaySound2D(impactSFX, "SFX", 0.15f, true);
        //screen red
        if (IsServer)
        {
            currentHealth.Value = Mathf.Clamp(currentHealth.Value - dmg, 0, maxHealth);
            //Debug.Log($"Server applied {dmg} damage to Player {OwnerClientId}");
        }
        else
        {

            TakeDamageServerRpc(dmg);
        }

        if (isAlive && currentHealth.Value <= 0)
        {
            isAlive = false;
            KillPlayer();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(int dmg)
    {
        currentHealth.Value = Mathf.Clamp(currentHealth.Value - dmg, 0, maxHealth);
        //Debug.Log($"Server applied {dmg} damage via RPC to Player {OwnerClientId}");
    }

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

    void KillPlayer()
    {
        //death sfx
        //death screen
        StartCoroutine(terminalTyper.TypeLines(outputText, deathScript, deathBGImage));
        Debug.Log("player died");
        SoundManager.Instance.StopMusic();

        isAlive = false; //you are now in death state
        if (PlayerManager.Instance.CheckAllPlayersStatus()) return;
        ResetPlayer();
    }

    public void MovePlayerToOutpost()
    {
        int index = Random.Range(0, GameManager.Instance.outpostSpawnPoints.Length);
        Vector3 newPosition = GameManager.Instance.outpostSpawnPoints[index].position;
        GetComponent<NetworkObject>().transform.position = newPosition;
    }

    public int GetWalletAmount()
    {
        walletAmount = GetComponent<PlayerWallet>().money.Value;
        return walletAmount;
    }

    public bool SetWalletAmount(int amount)
    {
        walletAmount = amount;
        GetComponent<PlayerWallet>().money.Value = walletAmount;

        return true;
    }

    public void SetHealth(int amount)
    {
        currentHealth.Value = amount;
    }

    public void SetStamina(float amount)
    {
        currentStamina = amount;
    }

    public void ResetPlayer() //called when ONE player dies
    {
        Debug.Log("Resetting Player");
        SetWalletAmount(0);
        SetHealth(health);
        SetStamina(stamina);
        iconGenerator.ResetInventoryIcons();
        playerInventory.ResetInventory();

        //isAlive = true; //depends on what a "dead" player means
        inOutpost = true;

        MovePlayerToOutpost();

        Debug.Log("Finished Resetting Player");
    }
}