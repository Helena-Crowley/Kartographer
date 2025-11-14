using UnityEngine;

public class HeadCameraSync : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    void Update()
    {
        if (cameraTransform != null)
            transform.rotation = cameraTransform.rotation; // world space
    }

}
