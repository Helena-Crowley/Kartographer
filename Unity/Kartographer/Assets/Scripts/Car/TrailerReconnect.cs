using UnityEngine;

public class TrailerReconnect : MonoBehaviour
{
    [Header("Cart Settings")]
    public Rigidbody cartRigidbody; // assign your GolfCart Rigidbody in the Inspector
    private Rigidbody trailerRb;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the trailer
        if (other.CompareTag("Trailer")) // <-- make sure your trailer is tagged "Trailer"
        {
            trailerRb = other.attachedRigidbody;
            if (trailerRb == null)
            {
                Debug.LogWarning("No Rigidbody found on trailer!");
                return;
            }


            // Check if it already has a ConfigurableJoint
            ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
            if (joint == null)
            {
                trailerRb.gameObject.transform.localPosition = new Vector3(0.03f, 0.14f, -5.56f);
                trailerRb.gameObject.transform.rotation = transform.rotation;
                joint = gameObject.AddComponent<ConfigurableJoint>();
            }
            else return;


            // Connect to the cart
            joint.connectedBody = trailerRb;
            joint.autoConfigureConnectedAnchor = false;

            // === Set Anchors ===
            joint.anchor = new Vector3(0f, 0.15f, -5.58f);
            joint.connectedAnchor = Vector3.zero;

            // === Linear Motion ===
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            // === Angular Motion ===
            joint.angularXMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularZMotion = ConfigurableJointMotion.Limited;

            // === Angular Limits ===
            SoftJointLimit lowX = joint.lowAngularXLimit;
            lowX.limit = -52f;
            joint.lowAngularXLimit = lowX;

            SoftJointLimit highX = joint.highAngularXLimit;
            highX.limit = 59f;
            joint.highAngularXLimit = highX;

            SoftJointLimit yLimit = joint.angularYLimit;
            yLimit.limit = 56f;
            joint.angularYLimit = yLimit;

            SoftJointLimit zLimit = joint.angularZLimit;
            zLimit.limit = 45f;
            joint.angularZLimit = zLimit;

            // === Angular Drives ===
            JointDrive xDrive = new JointDrive();
            xDrive.positionSpring = 4000f;
            xDrive.positionDamper = 400f;
            xDrive.maximumForce = Mathf.Infinity;
            joint.angularXDrive = xDrive;

            JointDrive yzDrive = new JointDrive();
            yzDrive.positionSpring = 3000f;
            yzDrive.positionDamper = 300f;
            yzDrive.maximumForce = Mathf.Infinity;
            joint.angularYZDrive = yzDrive;

            // === Break Forces ===
            joint.breakForce = 3000f;
            joint.breakTorque = 3000f;

            Debug.Log("Trailer successfully hitched to cart!");
        }
    }

    void OnJointBreak(float breakForce)
    {
        Debug.Log("trailer broke off!");
        trailerRb.linearVelocity = Vector3.zero;
        //trailerRb.AddExplosionForce(5000, transform.position, 50);
    }


}
