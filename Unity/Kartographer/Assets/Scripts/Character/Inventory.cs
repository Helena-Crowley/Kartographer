using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();

    public void Add(ItemData item)
    {
        items.Add(item);
        Debug.Log($"{gameObject.name} added {item.displayName}");
    }

    public bool Remove(ItemData item)
    {
        return items.Remove(item);
    }

    public ItemData GetLastItem()
    {
        if (items.Count > 0)
            return items[items.Count - 1]; // return the last added item
        return null;
    }
}
