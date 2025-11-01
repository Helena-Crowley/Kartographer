using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Camera mapCamera;
    private RenderTexture renderTexture;
    public RenderTexture mapTexture;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderTexture = mapCamera.GetComponent<RenderTexture>();
        renderTexture = mapTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
