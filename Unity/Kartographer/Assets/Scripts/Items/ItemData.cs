using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;       // unique ID
    public string displayName;  // what shows in UI
    //public Sprite icon;         // for inventory UI
    public GameObject prefab;   // optional: the 3D model or pickup prefab
    public int value;           // gold, worth, etc.
    public string description;  // tooltip text
    public bool isStackable;
    public int scale = 100;

    [HideInInspector]
    public Vector3 defaultScale;  

    private void OnValidate()
    {
        // Automatically update defaultScale when 'scale' is changed in Inspector
        defaultScale = new Vector3(scale, scale, scale);
    }
}
