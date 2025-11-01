using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();

    public int Add(ItemData item)
    {
        items.Add(item);
        //Debug.Log($"{gameObject.name} added {item.displayName}");
        return items.Count - 1;
    }

    public bool Remove(ItemData item)
    {
        return items.Remove(item);
    }

    public ItemData RemoveLast(out int slot)
    {
        if (items.Count == 0)
        {
            slot = -1;
            return null;
        }

        slot = items.Count - 1;      // last occupied slot
        ItemData last = items[slot];
        items.RemoveAt(slot);         // remove from inventory
        return last;
    }

    public ItemData GetLastItem()
    {
        if (items.Count > 0)
            return items[items.Count - 1]; // return the last added item
        return null;
    }
}
