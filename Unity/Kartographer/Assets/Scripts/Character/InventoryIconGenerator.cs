using UnityEngine;
using UnityEngine.UI;

public class InventoryIconGenerator : MonoBehaviour
{
    public Image[] inventorySlots;       // Your 5-box inventory UI

    // Generates an icon from a 3D prefab and sets it to a slot
    public void GenerateIcon(ItemData itemData, int slotIndex)
    {
        if (itemData.prefab == null || slotIndex < 0 || slotIndex >= inventorySlots.Length)
            return;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i == slotIndex)
            {
                inventorySlots[i].sprite = itemData.icon;
                inventorySlots[i].color = Color.white;
            }
        }

    }

    public int GetNextAvailableSlot()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].sprite == null) // empty slot
                return i;
        }
        return -1; // no free slot
    }

    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length)
            return;

        inventorySlots[slotIndex].sprite = null;
        inventorySlots[slotIndex].color = new Color(1, 1, 1, 0); // make transparent
    }
}
