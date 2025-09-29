using UnityEngine;
public class GroundedFeet : MonoBehaviour
{
    public Transform[] toePositions;
    private Vector3 hitPoint;
    private Vector3[] groundedPosition = new Vector3[4];
    private bool[] grounded = new bool[4];

    public Vector3[] bodyOffset = new Vector3[4];
    public float stepDistance = 2f;
    private Vector3[] targetStepPosition = new Vector3[4];

    public float forwardStepOffset = 0.7f;
    public float stepSpeed = 0.25f;
    public Rigidbody rb;

    void Start()
    {
        for (int i = 0; i < toePositions.Length; i++)
        {
            toePositions[i].position = CastRay(toePositions[i].position) + Vector3.up * 0.5f;
            grounded[i] = true;
        }
        toePositions[0].position = CastRay(transform.position + bodyOffset[0] + transform.forward * 5f) + Vector3.up * 0.5f;
        toePositions[2].position = CastRay(transform.position + bodyOffset[2] + transform.forward * 5f) + Vector3.up * 0.5f;

        for (int i = 0; i < toePositions.Length; i++)
        {
            groundedPosition[i] = toePositions[i].position;
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < toePositions.Length; i++)
        {
            if (i % 2 == 0 && grounded[1] && grounded[3])
            {
                UpdateStepPosition(i);
            }
            else if (i % 2 != 0 && grounded[0] && grounded[2])
            {
                UpdateStepPosition(i);
            }

            //toePositions[i].position = targetStepPosition;
            toePositions[i].position = Vector3.MoveTowards(toePositions[i].position, targetStepPosition[i], stepSpeed);
            Debug.Log(Vector3.Distance(toePositions[i].position, targetStepPosition[i]));
            if (Vector3.Distance(toePositions[i].position, targetStepPosition[i]) < 0.1f)
            {
                grounded[i] = true;
                //groundedPosition[i] = targetStepPosition[i];
            }
            groundedPosition[i] = targetStepPosition[i];


        }
    }

    Vector3 CastRay(Vector3 origin)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin + Vector3.up * 0.5f, Vector3.down, out hit, 50f))
        {
            return hit.point;
        }
        else
        {
            Debug.Log("No ground below toe");
            return Vector3.zero;
        }
    }



    void UpdateStepPosition(int i)
    {
        //if the distance btwn toe and target is big, target step updates
        Vector3 targetPoint = CastRay(transform.position + bodyOffset[i]);
        //Debug.DrawLine(transform.position + bodyOffset[i], targetPoint, Color.red);

        Debug.DrawLine(toePositions[i].position, targetPoint, Color.cyan);

        if (Vector3.Distance(toePositions[i].position, targetPoint) > stepDistance)
        {
            if (rb.linearVelocity.z < 0.1f)
            {
                forwardStepOffset = -forwardStepOffset;
            }
            targetStepPosition[i] = targetPoint + transform.forward * forwardStepOffset + Vector3.up * 0.5f;
            grounded[i] = false;
        }
        else
        {
            targetStepPosition[i] = groundedPosition[i];
        }
        Debug.DrawLine(targetStepPosition[i] + Vector3.up * 1, targetStepPosition[i], Color.magenta, 1f);
    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < toePositions.Length; i++)
        {
            Gizmos.DrawSphere(toePositions[i].position, 0.1f);
            Gizmos.DrawLine(transform.position + bodyOffset[i], targetStepPosition[i]);
        }
    
        

}

}

//if leg can go, update target step, otherwise stay put