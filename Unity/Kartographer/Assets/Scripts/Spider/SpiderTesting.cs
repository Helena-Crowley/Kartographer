using UnityEngine;

public class SpiderTesting : MonoBehaviour
{
    [System.Serializable]
    public class Foot
    {
        public GameObject footTarget;
        public Vector3 bodyOffset;

        [HideInInspector] public Vector3 initialPosition;
        public Transform targetPosition;
        [HideInInspector] public bool isGrounded = true;
    }

    public Foot[] feet;
    public float rayCastHeightOffset = 2f;
    public LayerMask groundLayer;


    void Start()
    {

        RaycastHit hit;
        for (int i = 0; i < 1; i++)
        {
            Debug.DrawRay(feet[i].footTarget.transform.position + feet[i].bodyOffset, Vector3.down * rayCastHeightOffset * 2, Color.red, 10f);
            if (Physics.Raycast(feet[i].footTarget.transform.position + feet[i].bodyOffset, Vector3.down * rayCastHeightOffset * 2, out hit, groundLayer))
            {
                feet[i].initialPosition = hit.point;
                //feet[i].targetPosition.position = hit.point;
                feet[i].footTarget.transform.position = hit.point;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < 1; i++)
        {
            UpdateFootPosition(feet[i]);
        }
    }
    void UpdateFootPosition(Foot foot)
    {
        RaycastHit hit;
        if (Vector3.Distance(foot.footTarget.transform.position, foot.initialPosition) > 2f)
        {
            foot.isGrounded = false;
            // Move foot to new position
            if (Physics.Raycast(foot.footTarget.transform.position + Vector3.up * rayCastHeightOffset, Vector3.down, out hit, rayCastHeightOffset * 2, groundLayer))
            {
                foot.initialPosition = hit.point;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (feet == null) return;

        Gizmos.color = Color.blue;
        for (int i = 0; i < feet.Length; i++)
        {
            if (feet[i].footTarget != null)
            {
               Gizmos.DrawSphere(feet[i].initialPosition, .1f);
            }
        }
    }
}
