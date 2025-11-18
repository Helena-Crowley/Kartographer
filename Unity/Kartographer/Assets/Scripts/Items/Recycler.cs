using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Recycler : MonoBehaviour
{
    public AudioClip destroySoundEffect;
    public TextMeshProUGUI amountText;
    public SellItems sellItemsObject;
    private List<GameObject> coins = new List<GameObject>();

    public GameObject coinPrefab;
    public Transform coinSpawn;

    private int totalValue;

    void Start()
    {
        amountText.text = "$0";
    }

    void OnTriggerEnter(Collider other)
    {
        // Validate tag
        if (!other.transform.root.CompareTag("Scrap"))
        {
            Debug.Log("This isn't trash!");
            return;
        }

        // Retrieve item data
        PickUppableItem item = other.transform.root.GetComponent<PickUppableItem>();
        if (item == null || item.itemData == null)
        {
            Debug.LogWarning("Scrap item missing PickUppableItem or itemData.");
            return;
        }

        // Add item value
        totalValue += item.itemData.value;
        Debug.Log("added item value");
        amountText.text = "$" + totalValue;

        for (int i = 0; i < item.itemData.value; i++)
        {
            Vector3 spawnOffset = new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(0.05f, 0.15f),
                Random.Range(-0.1f, 0.1f)
            );

            GameObject temp = Instantiate(coinPrefab, coinSpawn.position + spawnOffset, Quaternion.identity);
            coins.Add(temp);

            // Give them a tiny physics bounce if they have rigidbody
            Rigidbody rb = temp.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.AddForce(Vector3.up * Random.Range(1.5f, 2.5f), ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
            }
        }

        if (destroySoundEffect)
            SoundManager.Instance.PlaySound(destroySoundEffect, other.transform.position, "SFX", 0.5f, true);

        Destroy(other.gameObject, 0.5f);
    }

    void Update()
    {
        // If the player triggers a sell
        if (sellItemsObject != null && sellItemsObject.sellItems && totalValue > 0)
        {
            // Update player wallet
            if (sellItemsObject.PlayerWalletRef != null)
            {
                sellItemsObject.PlayerWalletRef.AddMoney(totalValue);
            }

            Debug.Log($"Sold scrap for ${totalValue}");

            // Reset recycler
            totalValue = 0;
            amountText.text = "$0";
            sellItemsObject.sellItems = false;

            foreach (GameObject coin in coins)
            {
                float randomTime = Random.Range(.5f, 1f);
                SoundManager.Instance.PlaySound(destroySoundEffect, coin.transform.position, "SFX", 0.3f, true);
                Destroy(coin, randomTime);
            }
        }
    }
}
