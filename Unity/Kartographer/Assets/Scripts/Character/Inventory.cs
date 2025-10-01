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
}
