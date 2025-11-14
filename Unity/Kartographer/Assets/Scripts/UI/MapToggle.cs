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

        Vector3 playerForward = transform.forward;

        Vector3 localForward = map.transform.InverseTransformDirection(playerForward);

        Vector3 projectedForward = new Vector3(localForward.x, 0, localForward.z).normalized;
        playerIcon.transform.localRotation = Quaternion.LookRotation(projectedForward, playerIcon.transform.up);
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
        Vector3 levelMin = GameManager.Instance.worldCenter - new Vector3(GameManager.Instance.worldXWidth, 0, GameManager.Instance.worldZWidth);
        Vector3 levelMax = GameManager.Instance.worldCenter + new Vector3(GameManager.Instance.worldXWidth, 0, GameManager.Instance.worldZWidth);

        float normalizedX = Mathf.InverseLerp(levelMin.x, levelMax.x, worldPos.x);
        float normalizedZ = Mathf.InverseLerp(levelMin.z, levelMax.z, worldPos.z);

        MeshRenderer mapRenderer = map.GetComponent<MeshRenderer>();
        float mapWidth = mapRenderer.bounds.extents.x / map.transform.lossyScale.x;
        float mapDepth = mapRenderer.bounds.extents.z / map.transform.lossyScale.z;

        return new Vector3(
            (normalizedX - 0.5f) * mapWidth,
            0.1f,
            (normalizedZ - 0.5f) * mapDepth
        );
    }




}