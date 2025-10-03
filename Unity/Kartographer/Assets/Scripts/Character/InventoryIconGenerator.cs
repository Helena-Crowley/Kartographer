using UnityEngine;
using UnityEngine.UI;

public class InventoryIconGenerator : MonoBehaviour
{
    public Image[] inventorySlots;       // Your 5-box inventory UI
    public Sprite offHandIcon; // assign your "OffHandIcon" in the inspector

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

    public bool IsSlotEmpty(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length)
            return true; // treat invalid index as empty

        Image slot = inventorySlots[slotIndex];
        return slot.sprite == null || slot.sprite == offHandIcon;
    }

    public int GetNextAvailableSlot()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (IsSlotEmpty(i))
                return i;
        }
        return -1; // no free slot
    }


    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length)
            return;

        inventorySlots[slotIndex].sprite = offHandIcon;
        inventorySlots[slotIndex].color = new Color(1, 1, 1, 1); // make transparent
    }
}
