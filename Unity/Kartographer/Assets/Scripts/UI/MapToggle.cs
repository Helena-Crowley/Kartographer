// // using UnityEngine;
// // using UnityEngine.InputSystem;

// // public class MapToggle : MonoBehaviour
// // {
// //     [Header("Map Settings")]
// //     public InputActionReference mapToggle;
// //     public GameObject map;             // The floating map in front of the player
// //     private bool mapEnabled = false;

// //     [Header("Icon Settings")]
// //     public GameObject building;        // The building to track
// //     public GameObject iconPrefab;      // Prefab for the icon
// //     private GameObject iconInstance;   // Instantiated icon
// //     public float bounceDuration = 0.6f;
// //     public AnimationCurve bounceCurve;

// //     [Header("Level Bounds")]
// //     public Vector3 levelMin = new Vector3(-50, 0, -50);
// //     public Vector3 levelMax = new Vector3(50, 0, 50);

// //     private Vector3 startPos;
// //     private Vector3 targetPos;
// //     private float t;
// //     private bool isMoving = false;

// //     void Start()
// //     {
// //         map.SetActive(false);

// //         // Create a default bounce curve if none is assigned
// //         if (bounceCurve == null)
// //         {
// //             bounceCurve = new AnimationCurve(
// //                 new Keyframe(0, 0),
// //                 new Keyframe(0.6f, 1.2f),
// //                 new Keyframe(1, 1)
// //             );
// //         }

// //         // Instantiate the icon but keep it hidden initially

// //     }

// //     void Update()
// //     {
// //         // Toggle the map
// //         if (mapToggle.action.WasPressedThisFrame())
// //         {
// //             if (iconPrefab != null && building != null)
// //             {
// //                 iconInstance = Instantiate(iconPrefab, map.transform);
// //                 iconInstance.SetActive(false);
// //             }
// //             mapEnabled = !mapEnabled;
// //             map.SetActive(mapEnabled);

// //             if (mapEnabled)
// //             {
// //                 if (iconInstance != null)
// //                 {
// //                     // Compute target position on the map
// //                     targetPos = GetIconLocalPosition(building.transform.position);

// //                     // Start below the target for bounce
// //                     startPos = targetPos - Vector3.up * 0.2f;
// //                     iconInstance.transform.localPosition = startPos;
// //                     iconInstance.SetActive(true);

// //                     // Start animation
// //                     t = 0;
// //                     isMoving = true;
// //                 }
// //             }
// //             else
// //             {
// //                 if (iconInstance != null)
// //                 {
// //                     // Bounce icon down before hiding
// //                     startPos = iconInstance.transform.localPosition;
// //                     targetPos = startPos - Vector3.up * 0.2f;
// //                     t = 0;
// //                     isMoving = true;
// //                 }
// //             }
// //         }

// //         // Animate the icon
// //         if (isMoving && iconInstance != null)
// //         {
// //             t += Time.deltaTime / bounceDuration;
// //             float curveValue = bounceCurve.Evaluate(t);
// //             iconInstance.transform.localPosition = Vector3.Lerp(startPos, targetPos, curveValue);

// //             if (t >= 1f)
// //             {
// //                 isMoving = false;

// //                 if (!mapEnabled)
// //                     iconInstance.SetActive(false);
// //             }
// //         }
// //     }

// //     // Converts a world position to the local map coordinates (0-1 normalized)
// //     Vector3 GetIconLocalPosition(Vector3 worldPos)
// //     {
// //         float normalizedX = (worldPos.x - levelMin.x) / (levelMax.x - levelMin.x);
// //         float normalizedZ = (worldPos.z - levelMin.z) / (levelMax.z - levelMin.z);

// //         // Map to local coordinates of the map plane (-0.5 to 0.5 range)
// //         Vector3 localPos = new Vector3(
// //             Mathf.Lerp(-0.5f, 0.5f, normalizedX),
// //             iconInstance != null ? iconInstance.transform.localPosition.y : 0.0f,
// //             Mathf.Lerp(-0.5f, 0.5f, normalizedZ)
// //         );

// //         return localPos;
// //     }
// // }
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class MapToggle : MonoBehaviour
// {
//     [Header("Map Settings")]
//     public InputActionReference mapToggle;
//     public GameObject map;             // The floating map in front of the player
//     private bool mapEnabled = false;

//     [Header("Icon Settings")]
//     public GameObject building;        // The building to track
//     public GameObject iconPrefab;      // Prefab for the icon
//     private GameObject iconInstance;   // Instantiated icon
//     public float bounceDuration = 0.6f;
//     public AnimationCurve bounceCurve;

//     [Header("Level Bounds")]
//     public Vector3 levelMin = new Vector3(-50, 0, -50);
//     public Vector3 levelMax = new Vector3(50, 0, 50);

//     private Vector3 startPos;
//     private Vector3 targetPos;
//     private float t;
//     private bool isMoving = false;

//     void Start()
//     {
//         map.SetActive(false);

//         if (bounceCurve == null)
//         {
//             bounceCurve = new AnimationCurve(
//                 new Keyframe(0, 0),
//                 new Keyframe(0.6f, 1.2f),
//                 new Keyframe(1, 1)
//             );
//         }

//         // Subscribe to scan complete
//         ScannerUI scannerUI = GetComponentInChildren<ScannerUI>();
//         if (scannerUI != null)
//         {
//             scannerUI.ScanCompleteEvent += OnBuildingScanned;
//             Debug.Log("Subscribed!");
//         }
//     }

//     void Update()
//     {
//         // Toggle map
//         if (mapToggle.action.WasPressedThisFrame())
//         {
//             mapEnabled = !mapEnabled;
//             map.SetActive(mapEnabled);
//         }

//         // Animate the icon if needed
//         if (isMoving && iconInstance != null)
//         {
//             t += Time.deltaTime / bounceDuration;
//             float curveValue = bounceCurve.Evaluate(t);
//             iconInstance.transform.localPosition = Vector3.Lerp(startPos, targetPos, curveValue);

//             if (t >= 1f)
//             {
//                 isMoving = false;
//             }
//         }
//     }

//     // This is called only when the building scan is complete
//     private void OnBuildingScanned()
//     {
//         if (iconInstance == null && iconPrefab != null)
//         {
//             iconInstance = Instantiate(iconPrefab, map.transform);
//             iconInstance.SetActive(true);
//         }

//         if (iconInstance != null && mapEnabled)
//         {
//             // Compute target position on the map
//             targetPos = GetIconLocalPosition(building.transform.position);

//             // Start below the target for bounce
//             startPos = targetPos - Vector3.up * 0.2f;
//             iconInstance.transform.localPosition = startPos;


//             // Start animation
//             t = 0;
//             isMoving = true;
//         }
//     }

//     private Vector3 GetIconLocalPosition(Vector3 worldPos)
//     {
//         float normalizedX = (worldPos.x - levelMin.x) / (levelMax.x - levelMin.x);
//         float normalizedZ = (worldPos.z - levelMin.z) / (levelMax.z - levelMin.z);

//         float iconHeight = 0.1f; // Adjust so it floats above map
//         return new Vector3(
//             Mathf.Lerp(-0.5f, 0.5f, normalizedX) * map.transform.localScale.x,
//             iconHeight,
//             Mathf.Lerp(-0.5f, 0.5f, normalizedZ) * map.transform.localScale.z
//         );
//     }
// }
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using LRS;

public class MapToggle : MonoBehaviour
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
