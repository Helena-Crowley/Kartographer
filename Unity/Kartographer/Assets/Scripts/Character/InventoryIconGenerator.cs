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
}
