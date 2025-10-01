using UnityEngine;
using UnityEngine.UI;

public class InventoryIconGenerator : MonoBehaviour
{
    public Camera iconCamera;            // The camera rendering the mesh
    public RenderTexture renderTexture;  // The RenderTexture assigned to the camera
    public Image[] inventorySlots;       // Your 5-box inventory UI

    // Generates an icon from a 3D prefab and sets it to a slot
    public void GenerateIcon(ItemData itemData, int slotIndex)
    {
        if (itemData.prefab == null || slotIndex < 0 || slotIndex >= inventorySlots.Length)
            return;

        // Instantiate prefab temporarily
        GameObject temp = Instantiate(itemData.prefab);
        temp.layer = LayerMask.NameToLayer("ItemIcon"); // make it visible only to the icon camera

        // Center it in front of the camera
        temp.transform.position = iconCamera.transform.position + iconCamera.transform.forward * 2f;
        temp.transform.rotation = Quaternion.identity;
        temp.transform.localScale = itemData.defaultScale;

        // Render the camera to the RenderTexture
        iconCamera.Render();

        // Read pixels from RenderTexture into a Texture2D
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        // Convert Texture2D into a Sprite
        Sprite icon = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        Debug.Log(icon != null ? "Sprite generated" : "Sprite null!");

        // Assign the sprite to the inventory slot
        inventorySlots[slotIndex].sprite = icon;
        inventorySlots[slotIndex].enabled = true;

        // Clean up
        Destroy(temp);
    }
}
