using UnityEngine;
public class CameraManager : MonoBehaviour
{
    public Camera playerCamera;
    public Camera cartCamera;

    void Start()
    {
        SwitchToPlayerCamera();
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
        InputManager.OnCartStateChanged += HandleCartStateChanged;
    }

    private void OnDisable()
    {
        InputManager.OnCartStateChanged -= HandleCartStateChanged;
    }

    private void HandleCartStateChanged(bool inCart)
    {
        if (inCart)
            SwitchToCartCamera();
        else
            SwitchToPlayerCamera();
    }
}
