using UnityEngine;

public class TireAttach : MonoBehaviour
{
    public enum WheelLocation
    {
        frontRight,
        frontLeft,
        rearRight,
        rearLeft
    }

    [Tooltip("put the tire mesh that's deactivated on the cart prefab right ere")]
    [SerializeField] private Transform tirePos;
    public Collider wheelInteractionCollider;
    public WheelLocation wheelLocation;

    public CarMovement carMovement;

    public void AttachTireMesh(GameObject tire)
    {
        Debug.Log("Entered attacn tire mesh function");
        if (!wheelInteractionCollider.enabled) return;

        Debug.Log("attempting to parent and attach tire!");

        tire.transform.SetParent(gameObject.transform);
        tire.transform.position = tirePos.position;
        wheelInteractionCollider.enabled = false;

        carMovement.ToggleWheel(wheelLocation, wheelInteractionCollider);

        //got up to this point, just need ot tie in that if the collider is false then the wheel is active and the physdics update should run
    }
}
