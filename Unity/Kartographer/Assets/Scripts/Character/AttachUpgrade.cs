using UnityEngine;

public class AttachUpgrade : MonoBehaviour
{
    public GameObject pickedUpTire;

    [SerializeField] private GameObject cam;
    [SerializeField] private LayerMask upgradeLayer;
    [SerializeField] private float maxDistance = 5f;

    public TireAttach tireAttach;

    void FixedUpdate()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 200, Color.blue);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, upgradeLayer))
        {
            Debug.Log("Entered Attach uipgrade");
            tireAttach = hit.collider.GetComponentInParent<TireAttach>();
            if (tireAttach == null) return;

            Debug.Log("Calling AttachTireMesh()");
            tireAttach.AttachTireMesh(pickedUpTire);
        }
    }
}
