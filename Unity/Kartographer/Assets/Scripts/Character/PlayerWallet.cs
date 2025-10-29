using UnityEngine;
using TMPro;
using Unity.Netcode;

public class PlayerWallet : NetworkBehaviour
{
    public TextMeshProUGUI moneyText;

    // NetworkVariable: automatically syncs value between server and clients
    private NetworkVariable<int> money = new NetworkVariable<int>(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    private void Start()
    {
        UpdateMoneyUI();
        money.OnValueChanged += OnMoneyChanged;
    }

    private void OnDestroy()
    {
        money.OnValueChanged -= OnMoneyChanged;
    }

    private void OnMoneyChanged(int oldValue, int newValue)
    {
        UpdateMoneyUI();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        if (!IsServer)
        {
            // Clients can’t directly modify money — ask the server
            AddMoneyServerRpc(amount);
        }
        else
        {
            money.Value += amount;
            UpdateMoneyUI();
        }
    }

    [ServerRpc]
    private void AddMoneyServerRpc(int amount)
    {
        money.Value += amount;
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = "$" + money.Value;
    }
}
