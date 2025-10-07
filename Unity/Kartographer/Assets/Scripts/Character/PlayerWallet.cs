using UnityEngine;
using TMPro; // assuming your UI text uses TextMeshPro

public class PlayerWallet : MonoBehaviour
{
    public int money = 0;
    public TextMeshProUGUI moneyText;

    private void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = "$" + money.ToString();
    }
}
