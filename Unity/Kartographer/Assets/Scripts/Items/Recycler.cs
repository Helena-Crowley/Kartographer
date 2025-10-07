using UnityEngine;

public class Recycler : MonoBehaviour
{
    private PickUppableItem depositedItem;
    private int moneyToReturn;

    void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.CompareTag("Scrap"))
        {
            Debug.Log("This isn't trash wth???");
            return;
        }

        PickUppableItem depositedItem = other.transform.root.GetComponent<PickUppableItem>();
        if (depositedItem == null || depositedItem.itemData == null)
        {
            Debug.LogWarning("Scrap item missing PickUppableItem or itemData.");
            return;
        }

        int value = depositedItem.itemData.value;
        //moneyToReturn += value;

        Debug.Log($"Deposited scrap worth ${value}. Total: ${moneyToReturn}");

        Debug.Log(depositedItem.itemData.owner);

        if (depositedItem.itemData.owner != null)
        {
            PlayerWallet wallet = depositedItem.itemData.owner.GetComponent<PlayerWallet>();
            if (wallet != null)
                wallet.AddMoney(value);
        }

        Destroy(other.gameObject, 1f);
    }
}
