// using UnityEngine;

// public class FogPainter : MonoBehaviour
// {
//     [Header("References")]
//     public RenderTexture fogMask;    // Global fog mask (full world)
//     public Material paintMaterial;   // Custom/FogPaint
//     [Header("World Bounds")]
//     public float worldMinX = -100f;
//     public float worldMaxX = 100f;
//     public float worldMinZ = -100f;
//     public float worldMaxZ = 100f;

//     [Header("Brush Settings")]
//     [Range(0.01f, 0.2f)] public float brushSize = 0.05f;

//     private RenderTexture ping;
//     private RenderTexture pong;

//     public Camera minimapCamera;

//     private void Start()
//     {
//         if (fogMask == null || paintMaterial == null)
//         {
//             Debug.LogError("FogPainter missing references!");
//             enabled = false;
//             return;
//         }

//         ping = new RenderTexture(fogMask.width, fogMask.height, 0, fogMask.format);
//         pong = new RenderTexture(fogMask.width, fogMask.height, 0, fogMask.format);
//         ping.Create();
//         pong.Create();

//         // Initialize to black (fogged)
//         Graphics.Blit(Texture2D.blackTexture, ping);
//         Graphics.Blit(Texture2D.blackTexture, pong);

//         // Copy initial state to fogMask
//         Graphics.Blit(ping, fogMask);
//     }

//     private void Update()
//     {
//         if (fogMask == null || paintMaterial == null)
//             return;

//         // Convert player world position to UV (0-1) over entire world
//         float uvX = Mathf.InverseLerp(worldMinX, worldMaxX, transform.position.x);
//         float uvY = Mathf.InverseLerp(worldMinZ, worldMaxZ, transform.position.z);

//         uvX = Mathf.Clamp01(uvX);
//         uvY = Mathf.Clamp01(uvY);

//         // Assign brush position and size
//         paintMaterial.SetVector("_BrushPos", new Vector4(uvX, uvY, 0, 0));
//         paintMaterial.SetFloat("_BrushSize", brushSize);

//         // Ping-pong
//         Graphics.Blit(ping, pong, paintMaterial);
//         var temp = ping; ping = pong; pong = temp;

//         // Update the global fog mask for minimap shader
//         Graphics.Blit(ping, fogMask);
//     }

//     private void OnDestroy()
//     {
//         if (ping != null) ping.Release();
//         if (pong != null) pong.Release();
//     }
// }
using UnityEngine;

public class FogPainter : MonoBehaviour
{
    [Header("References")]
    public RenderTexture fogMask;    // Global fog mask (large texture)
    public Material paintMaterial;   // Brush material (Custom/FogPaint)

    [Header("World Bounds (match MinimapFogController)")]
    public float worldMinX = -100f;
    public float worldMaxX = 100f;
    public float worldMinZ = -100f;
    public float worldMaxZ = 100f;

    [Header("Brush Settings")]
    [Range(0.01f, 0.2f)]
    public float brushSize = 0.05f; // relative to fogMask UV (0-1)

    private RenderTexture ping;
    private RenderTexture pong;

    public Camera minimapCamera;

    private void Start()
    {
        if (fogMask == null || paintMaterial == null)
        {
            Debug.LogError("FogPainter missing references!");
            enabled = false;
            return;
        }

        // Create double-buffer textures
        ping = new RenderTexture(fogMask.width, fogMask.height, 0, fogMask.format);
        pong = new RenderTexture(fogMask.width, fogMask.height, 0, fogMask.format);
        ping.Create();
        pong.Create();

        // Initialize fog to black
        Graphics.Blit(Texture2D.blackTexture, ping);
        Graphics.Blit(Texture2D.blackTexture, pong);

        // Copy initial state to the actual mask
        Graphics.Blit(ping, fogMask);
    }

    private void Update()
    {
        // Convert player world position to global fog UV
        float u = Mathf.InverseLerp(worldMinX, worldMaxX, transform.position.x);
        float v = Mathf.InverseLerp(worldMinZ, worldMaxZ, transform.position.z);

        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);

        paintMaterial.SetVector("_BrushPos", new Vector4(u, v, brushSize, 0));
        paintMaterial.SetTexture("_MainTex", ping); // previous fog state

        // Ping-pong double buffer
        Graphics.Blit(ping, pong, paintMaterial);
        var temp = ping;
        ping = pong;
        pong = temp;

        // Update global fog mask for shaders/UI
        Graphics.Blit(ping, fogMask);
    }

    private void OnDestroy()
    {
        if (ping != null) ping.Release();
        if (pong != null) pong.Release();
    }
}
