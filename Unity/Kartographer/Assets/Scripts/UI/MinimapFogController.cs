using UnityEngine;
using UnityEngine.UI;

public class MinimapFogController : MonoBehaviour
{
    [Header("Render Textures")]
    public RenderTexture minimapTexture;
    public RenderTexture fogMaskTexture;

    [Header("Shader (use MinimapFogRevealZoomed shader)")]
    public Shader minimapFogShader;

    [Header("Minimap Camera (for zoomed-in bounds)")]
    public Camera minimapCamera;

    [Header("World Bounds")]
    public float worldMinX = -100f;
    public float worldMaxX = 100f;
    public float worldMinZ = -100f;
    public float worldMaxZ = 100f;

    [Header("Settings")]
    public Color unexploredColor = Color.black;

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
            minimapFogShader = Shader.Find("Custom/MinimapFogRevealZoomed");

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
    }

    private void LateUpdate()
    {
        if (materialInstance == null || minimapCamera == null) return;

        // Update minimap camera bounds each frame
        float orthoSize = minimapCamera.orthographicSize;
        float aspect = minimapCamera.aspect;
        Vector3 camPos = minimapCamera.transform.position;

        Vector4 viewBounds = new Vector4(
            camPos.x - orthoSize * aspect, camPos.x + orthoSize * aspect,
            camPos.z - orthoSize, camPos.z + orthoSize
        );

        materialInstance.SetVector("_MinimapViewBounds", viewBounds);

        // Keep textures updated
        materialInstance.SetTexture("_MainTex", minimapTexture);
        materialInstance.SetTexture("_FogMask", fogMaskTexture);
    }

    private void OnDestroy()
    {
        if (materialInstance != null) Destroy(materialInstance);
    }
}
