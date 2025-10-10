using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    public ItemData[] floorItems;
    public ItemData[] shelfItems;
    public ItemData[] tableItems;
    public ItemData[] outdoorItems;

    [HideInInspector] public ItemData[] allItems;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        var combined = new List<ItemData>();
        if (floorItems != null) combined.AddRange(floorItems);
        if (shelfItems != null) combined.AddRange(shelfItems);
        if (tableItems != null) combined.AddRange(tableItems);
        if (outdoorItems != null) combined.AddRange(outdoorItems);

        allItems = combined.ToArray();
    }

    public int GetItemIndex(ItemData item)
    {
        for (int i = 0; i < allItems.Length; i++)
            if (allItems[i] == item)
                return i;
        return -1;
    }

    public ItemData GetItemById(int id)
    {
        if (id >= 0 && id < allItems.Length)
            return allItems[id];
        return null;
    }

}
