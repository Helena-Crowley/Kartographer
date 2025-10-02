using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    public ItemData[] floorItems;
    public ItemData[] shelfItems;
    public ItemData[] tableItems;
    public ItemData[] outdoorItems;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
