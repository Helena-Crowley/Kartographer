using UnityEngine;
using Unity.Netcode;

public class CameraManager : NetworkBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Camera cartCamera;
    private PlayerInputManager inputManager;

    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            SwitchToPlayerCamera();
            inputManager.OnCartStateChanged += HandleCartStateChanged;
        }
        else
        {
            // Ensure non-owners cameras are off
            if (playerCamera != null) playerCamera.enabled = false;
            if (cartCamera != null) cartCamera.enabled = false;
        }
    }

    private void OnDisable()
    {
        if (!IsOwner) return;
        inputManager.OnCartStateChanged -= HandleCartStateChanged;
    }

    public void HandleCartStateChanged(bool inCart)
    {
        if (inCart)
            SwitchToCartCamera();
        else
            SwitchToPlayerCamera();
    }

    public void SwitchToPlayerCamera()
    {
        playerCamera.enabled = true;
        cartCamera.enabled = false;
        cartCamera.GetComponent<CameraFollow>().enabled = false;
        //Debug.Log("Switched to Player Camera");
    }

    public void SwitchToCartCamera()
    {
        playerCamera.enabled = false;
        cartCamera.enabled = true;
        cartCamera.GetComponent<CameraFollow>().enabled = true;
        //Debug.Log("Switched to Cart Camera");
    }
}
