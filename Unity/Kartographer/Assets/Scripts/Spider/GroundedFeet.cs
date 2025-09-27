using UnityEngine;

public class GroundedFeet : MonoBehaviour
{
    public GameObject footTarget1;
    public GameObject footTarget2;
    public GameObject footTarget3;
    public GameObject footTarget4;

    public LayerMask groundLayer;

    public Vector3 bodyOffset1;

    private Vector3 lastStep;

    void Update()
    {
        StickFeetToGround(footTarget1, bodyOffset1);
        //StickFeetToGround(footTarget2, canStep2);
        //StickFeetToGround(footTarget3, canStep3);
        //StickFeetToGround(footTarget4, canStep4);
    }

    void StickFeetToGround(GameObject footTarget, Vector3 stepTargetOffset)
    {
        if (CanStep(footTarget, StepTargetCast(stepTargetOffset)))
        {
            RaycastHit hit;
            if (Physics.Raycast(footTarget.transform.position, Vector3.down, out hit, 10f, groundLayer))
            {
                Debug.DrawRay(footTarget.transform.position, Vector3.down * hit.distance, Color.red);
                //lastStep = hit.point + Vector3.up * .1f; // Slightly above ground
                lastStep = StepTargetCast(stepTargetOffset);
            }
            else
            {

                Debug.Log("No ground detected below footTarget");
            }
        }

        //footTarget.transform.position = lastStep + Vector3.up * 1f; // Slightly above ground
        footTarget.transform.position = Vector3.Lerp(footTarget.transform.position, lastStep + Vector3.up * .1f, Time.deltaTime * 10f);

    }

    bool CanStep(GameObject footTarget, Vector3 stepTarget)
    {
        Debug.DrawLine(footTarget.transform.position, stepTarget, Color.green);
        Debug.Log(Vector3.Distance(footTarget.transform.position, stepTarget));
        if (Vector3.Distance(footTarget.transform.position, stepTarget) > 1.9f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 StepTargetCast(Vector3 offset)
    {
        RaycastHit hit;
        if (Physics.Raycast(offset + transform.position, Vector3.down, out hit, 10f, groundLayer))
        {
            Debug.DrawRay(offset + transform.position, Vector3.down * hit.distance, Color.blue);
            return hit.point + Vector3.up * 0.1f; // Slightly above ground
        }
        else
        {
            Debug.Log("No ground detected below stepTargetCast");
            return Vector3.zero;
        }
    }
}
