using Unity.VisualScripting;
using UnityEngine;

public class GroundedFeet : MonoBehaviour
{
    public Transform[] toePositions;
    private Vector3 hitPoint;
    private Vector3[] groundedPosition = new Vector3[8];
    private bool[] grounded = new bool[8];

    public GameObject[] bodyOffset;
    public float stepDistance = 2f;
    private Vector3[] targetStepPosition = new Vector3[8];
    public GameObject tipForLength;

    public float forwardStepOffset = 0.7f;
    public float stepSpeed = 0.25f;
    public Rigidbody rb;

    public float bodyHeight = 2f;
    public float bodySpeed = 5f;

    private Quaternion lastRotation;
    public GameObject target;
    public float movementSpeed = 5f;
    public float rotateSpeed = 10f;
    private float footHeightOffset;

    void Start()
    {
        Renderer rend = tipForLength.GetComponent<Renderer>();
        footHeightOffset = rend != null ? rend.bounds.extents.y * 2 : 0;

        for (int i = 0; i < toePositions.Length; i++)
        {
            toePositions[i].position = CastRay(toePositions[i].position, footHeightOffset) + Vector3.up * 0.5f;
            //grounded[i] = true;
            groundedPosition[i] = toePositions[i].position;
            targetStepPosition[i] = toePositions[i].position;
        }

        lastRotation = transform.rotation;
        grounded[0] = false;
        grounded[2] = false;
        grounded[4] = false;
        grounded[6] = false;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < toePositions.Length; i++)
        {
            if (i % 2 == 0 && (grounded[1] && grounded[3]) && (grounded[5] && grounded[7]))
            {
                UpdateStepPosition(i);
            }
            else if (i % 2 != 0 && (grounded[0] && grounded[2]) && (grounded[4] && grounded[6]))
            {
                UpdateStepPosition(i);
            }

            if (Mathf.Abs(rb.rotation.y) - Mathf.Abs(lastRotation.y) > 0.2f)
            {
                UpdateStepPosition(i);
                lastRotation = rb.rotation;
            }

            toePositions[i].position = Vector3.MoveTowards(toePositions[i].position, targetStepPosition[i], stepSpeed);

            if (Vector3.Distance(toePositions[i].position, targetStepPosition[i]) < 0.1f)
            {
                grounded[i] = true;
            }

            groundedPosition[i] = targetStepPosition[i];
        }

        UpdateBodyPosition();
        FollowTarget();
    }

    Vector3 CastRay(Vector3 origin, float offset = 0)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin + Vector3.up * 0.5f, Vector3.down, out hit, 50f))
        {
            return hit.point + Vector3.up * offset;
        }
        else
        {
            Debug.Log("No ground below toe");
            return Vector3.zero;
        }
    }

    void UpdateStepPosition(int i)
    {
        Vector3 targetPoint = CastRay(bodyOffset[i].transform.position, footHeightOffset);
        Debug.DrawLine(bodyOffset[i].transform.position, targetPoint, Color.red);

        if (Vector3.Distance(toePositions[i].position, targetPoint) > stepDistance)
        {
            // if (rb.linearVelocity.z < 0.01f)
            // {
            //     forwardStepOffset = -forwardStepOffset;
            // }


            targetStepPosition[i] = CastRay(
                targetPoint + transform.forward * forwardStepOffset + Vector3.up * 5f,
                footHeightOffset
            );

            grounded[i] = false;
        }
        else
        {
            targetStepPosition[i] = groundedPosition[i];
        }

        Debug.DrawLine(toePositions[i].position, targetStepPosition[i], Color.magenta, 1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < toePositions.Length; i++)
        {
            Gizmos.DrawSphere(targetStepPosition[i], 0.1f);
        }
    }

    private void UpdateBodyPosition()
    {
        Vector3 averagePosition = Vector3.zero;
        for (int i = 0; i < toePositions.Length; i++)
        {
            averagePosition += toePositions[i].position;
        }
        averagePosition /= toePositions.Length;

        Vector3 targetPosition = new Vector3(
            transform.position.x,
            averagePosition.y + bodyHeight,
            transform.position.z
        );

        Vector3 moveDirection = (targetPosition - transform.position) * bodySpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + moveDirection);
    }

    private void FollowTarget()
    {
        Vector3 direction = (target.transform.position - rb.position).normalized;
        rb.MovePosition(rb.position + direction * movementSpeed * Time.fixedDeltaTime);

        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion smoothRotation = Quaternion.Lerp(rb.rotation, lookRotation, rotateSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothRotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.gameObject;
        }
    }
}
