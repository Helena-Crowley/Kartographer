using UnityEngine;
using UnityEngine.UI;

public class MinimapFogController : MonoBehaviour
{
    [Header("Render Textures")]
    public RenderTexture minimapTexture;
    public RenderTexture fogMaskTexture;

    [Header("Shader")]
    public Shader minimapFogShader;

    [Header("Minimap Camera (for zoomed-in bounds)")]
    public Camera minimapCamera;

    [Header("World Bounds (MUST match FogPainter)")]
    public float worldMinX = -100f;
    public float worldMaxX = 100f;
    public float worldMinZ = -100f;
    public float worldMaxZ = 100f;

    [Header("Settings")]
    public Color unexploredColor = Color.black;

    [Header("Debug")]
    public bool showDebugInfo = true;

    private RawImage rawImage;
    private Material materialInstance;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("No RawImage component found!");
            return;
        }

        if (minimapFogShader == null)
        {
            minimapFogShader = Shader.Find("Custom/MinimapFogReveal");
            if (minimapFogShader == null)
            {
                Debug.LogError("Could not find Custom/MinimapFogReveal shader!");
                return;
            }
        }

        SetupMinimapMaterial();
    }

    private void SetupMinimapMaterial()
    {
        materialInstance = new Material(minimapFogShader);

        materialInstance.SetTexture("_MainTex", minimapTexture);
        materialInstance.SetTexture("_FogMask", fogMaskTexture);
        materialInstance.SetColor("_UnexploredColor", unexploredColor);
        materialInstance.SetVector("_WorldBounds", new Vector4(worldMinX, worldMaxX, worldMinZ, worldMaxZ));

        rawImage.material = materialInstance;
        rawImage.texture = minimapTexture;

        if (showDebugInfo)
        {
            Debug.Log($"MinimapFogController: World bounds set to ({worldMinX}, {worldMaxX}, {worldMinZ}, {worldMaxZ})");
        }
    }

    private void LateUpdate()
    {
        if (materialInstance == null) { return; }
        if (minimapCamera == null) { return; }


        // Calculate what the camera is currently viewing
        float orthoSize = minimapCamera.orthographicSize;
        float aspect = minimapCamera.aspect;
        Vector3 camPos = minimapCamera.transform.position;

        // View bounds: what world coordinates are visible in the minimap
        // Remove the zoomFactor multiplication - just use the camera's natural orthographic size
        Vector4 viewBounds = new Vector4(
            camPos.x - orthoSize * aspect,
            camPos.x + orthoSize * aspect,
            camPos.z - orthoSize,
            camPos.z + orthoSize
        );
        materialInstance.SetVector("_MinimapViewBounds", viewBounds);


        // Keep textures updated
        materialInstance.SetTexture("_MainTex", minimapTexture);
        materialInstance.SetTexture("_FogMask", fogMaskTexture);

        if (showDebugInfo && Time.frameCount % 60 == 0) // Log once per second
        {
            Debug.Log($"Camera at ({camPos.x:F1}, {camPos.z:F1}) | View bounds: ({viewBounds.x:F1}, {viewBounds.y:F1}, {viewBounds.z:F1}, {viewBounds.w:F1})");

            // Calculate what fog mask UV this corresponds to
            float centerFogU = Mathf.InverseLerp(worldMinX, worldMaxX, camPos.x);
            float centerFogV = Mathf.InverseLerp(worldMinZ, worldMaxZ, camPos.z);
            Debug.Log($"Center of view maps to fog UV: ({centerFogU:F2}, {centerFogV:F2})");
        }
    }

    private void OnDestroy()
    {
        if (materialInstance != null) Destroy(materialInstance);
    }
}