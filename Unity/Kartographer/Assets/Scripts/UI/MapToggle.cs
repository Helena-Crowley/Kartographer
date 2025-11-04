using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using LRS;

public class MapToggle : NetworkBehaviour
{
    [Header("Map Settings")]
    public InputActionReference mapToggle;
    public GameObject map; // The floating map in front of the player

    [Header("Icon Settings")]
    public GameObject iconPrefab; // Prefab for the icons
    public GameObject playerIcon;

    [Header("Level Bounds")]
    public Vector3 levelMin = new Vector3(-50, 0, -50);
    public Vector3 levelMax = new Vector3(50, 0, 50);

    // Internal tracking
    private bool mapEnabled = false;
    public List<GameObject> buildings = new List<GameObject>();
    private Dictionary<GameObject, GameObject> icons = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        map.SetActive(false);

        // Subscribe to all ScannerUIs in the scene using the non-deprecated API
        foreach (var scannerUI in Object.FindObjectsByType<ScannerUI>(FindObjectsSortMode.None))
        {
            scannerUI.ScanCompleteEvent += OnBuildingScanned; // now correct
        }

        playerIcon = Instantiate(playerIcon, map.transform);
    }

    void Update()
    {
        if (!IsOwner) return;
        // Toggle map visibility
        if (mapToggle.action.WasPressedThisFrame())
        {
            mapEnabled = !mapEnabled;
            map.SetActive(mapEnabled);

            // Update all icons based on map state
            foreach (var icon in icons.Values)
            {
                icon.SetActive(mapEnabled);
            }
        }
        playerIcon.transform.localPosition = GetIconLocalPosition(transform.position);
    }

    public void OnBuildingScanned(GameObject building)
    {
        if (!icons.ContainsKey(building))
        {
            GameObject icon = Instantiate(iconPrefab, map.transform);
            icon.transform.localPosition = GetIconLocalPosition(building.transform.position);
            icon.SetActive(mapEnabled);
            icons.Add(building, icon);
        }
    }


    private Vector3 GetIconLocalPosition(Vector3 worldPos)
    {
        // Normalize positions to 0–1
        float normalizedX = Mathf.InverseLerp(levelMin.x, levelMax.x, worldPos.x);
        float normalizedZ = Mathf.InverseLerp(levelMin.z, levelMax.z, worldPos.z);

        // Get map dimensions in local space
        float localX = 0f;
        float localZ = 0f;

        MeshRenderer mapRenderer = map.GetComponent<MeshRenderer>();
        if (mapRenderer != null)
        {
            float mapWidth = mapRenderer.bounds.size.x / map.transform.lossyScale.x;
            float mapDepth = mapRenderer.bounds.size.z / map.transform.lossyScale.z;
            localX = (normalizedX - 0.5f) * mapWidth;
            localZ = (normalizedZ - 0.5f) * mapDepth;
        }

        float iconHeight = 0.1f; // floating slightly above map
        return new Vector3(localX, iconHeight, localZ);
    }


}
