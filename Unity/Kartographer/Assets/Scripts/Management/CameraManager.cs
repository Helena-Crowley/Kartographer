using UnityEngine;
public class CameraManager : MonoBehaviour
{
    public Camera playerCamera;
    public Camera cartCamera;

    private PlayerInputManager inputManager;

    void Start()
    {
        SwitchToPlayerCamera();

        inputManager = GetComponent<PlayerInputManager>();
    }

    public void SwitchToPlayerCamera()
    {
        playerCamera.enabled = true;
        cartCamera.enabled = false;
    }

    public void SwitchToCartCamera()
    {
        playerCamera.enabled = false;
        cartCamera.enabled = true;
    }

    private void OnEnable()
    {
        inputManager.OnCartStateChanged += HandleCartStateChanged;
    }

    private void OnDisable()
    {
        inputManager.OnCartStateChanged -= HandleCartStateChanged;
    }

    private void HandleCartStateChanged(bool inCart)
    {
        if (inCart)
            SwitchToCartCamera();
        else
            SwitchToPlayerCamera();
    }
}
