// using UnityEngine;

// public class FogPainter : MonoBehaviour
// {
//     [Header("References")]
//     public RenderTexture fogMask;    // Global fog mask (large texture)
//     public Material paintMaterial;   // Brush material (Custom/FogPaint)

//     [Header("World Bounds (match MinimapFogController)")]
//     public float worldMinX = -100f;
//     public float worldMaxX = 100f;
//     public float worldMinZ = -100f;
//     public float worldMaxZ = 100f;

//     [Header("Brush Settings")]
//     [Tooltip("Reveal radius in world units (e.g., 10 = 10 meter radius)")]
//     public float revealRadius = 10f;

//     [Header("Debug")]
//     public bool showDebugInfo = true;

//     private RenderTexture ping;
//     private RenderTexture pong;
//     public Camera minimapCamera;

//     private void Awake()
//     {
//         PlayerUIManager.Instance.BindPlayer(this);
//     }

//     private void Start()
//     {
//         if (fogMask == null || paintMaterial == null)
//         {
//             Debug.LogError("FogPainter missing references!");
//             enabled = false;
//             return;
//         }

//         // Create double-buffer textures
//         ping = new RenderTexture(fogMask.width, fogMask.height, 0, fogMask.format);
//         pong = new RenderTexture(fogMask.width, fogMask.height, 0, fogMask.format);
//         ping.Create();
//         pong.Create();

//         // Initialize fog to black
//         Graphics.Blit(Texture2D.blackTexture, ping);
//         Graphics.Blit(Texture2D.blackTexture, pong);

//         // Copy initial state to the actual mask
//         Graphics.Blit(ping, fogMask);
//     }

//     private void Update()
//     {
        
//         // Convert player world position to global fog UV (0-1 across entire world)
//         float u = Mathf.InverseLerp(worldMinX, worldMaxX, transform.position.x);
//         float v = Mathf.InverseLerp(worldMinZ, worldMaxZ, transform.position.z);

//         u = Mathf.Clamp01(u);
//         v = Mathf.Clamp01(v);

//         // Convert world-space reveal radius to normalized UV space
//         float worldWidth = worldMaxX - worldMinX;
//         float worldHeight = worldMaxZ - worldMinZ;
        
//         // Use the smaller dimension to keep the brush circular
//         float worldSize = Mathf.Min(worldWidth, worldHeight);
//         float normalizedBrushSize = revealRadius / worldSize;

//         // Send to shader - using Vector4.z for brush size
//         paintMaterial.SetVector("_BrushPos", new Vector4(u, v, normalizedBrushSize, 0));
//         paintMaterial.SetTexture("_MainTex", ping); // previous fog state

//         // Ping-pong double buffer
//         Graphics.Blit(ping, pong, paintMaterial);
//         var temp = ping;
//         ping = pong;
//         pong = temp;

//         // Update global fog mask
//         Graphics.Blit(ping, fogMask);
//     }

//     private void OnDestroy()
//     {
//         if (ping != null) ping.Release();
//         if (pong != null) pong.Release();
//     }
// }