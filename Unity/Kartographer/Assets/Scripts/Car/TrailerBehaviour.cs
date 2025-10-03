using Unity.VisualScripting;
using UnityEngine;

public class TrailerBehaviour : MonoBehaviour
{
    public Collider recconnectZone;
    public ConfigurableJoint joint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        recconnectZone.enabled = true;
        joint.connectedBody = null;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cart"))
        {
            Debug.Log("Cart entered reconnect zone");
            recconnectZone.enabled = false;
            joint.anchor = new Vector3(0, .55f, -2.24f);
            // Attach trailer to cart
            joint.connectedBody = other.GetComponent<Rigidbody>();

        }
    }
}
