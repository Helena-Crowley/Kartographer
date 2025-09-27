using UnityEngine;

public class GroundedFeet : MonoBehaviour
{
    public GameObject footTarget1;
    public GameObject footTarget2;
    public GameObject footTarget3;
    public GameObject footTarget4;

    public LayerMask groundLayer;

    public Vector3 bodyOffset1;
    public Vector3 bofdyOffset2;
    public Vector3 bodyOffset3;
    public Vector3 bodyOffset4;

    private Vector3 lastStep;

    void Update()
    {
        StickFeetToGround(footTarget1, bodyOffset1);
        StickFeetToGround(footTarget2, bofdyOffset2);
        StickFeetToGround(footTarget3, bodyOffset3);
        StickFeetToGround(footTarget4, bodyOffset4);
    }

    void StickFeetToGround(GameObject footTarget, Vector3 stepTargetOffset)
    {
        //lastStep = footTarget.transform.position;
        if (CanStep(footTarget, StepTargetCast(stepTargetOffset)))
        {
            RaycastHit hit;
            if (Physics.Raycast(footTarget.transform.position, Vector3.down, out hit, 10f, groundLayer))
            {
                Debug.DrawRay(footTarget.transform.position, Vector3.down * hit.distance, Color.red);
                //lastStep = hit.point + Vector3.up * .1f; // Slightly above ground
                lastStep = StepTargetCast(stepTargetOffset);
                footTarget.transform.position = Vector3.Lerp(footTarget.transform.position, lastStep + Vector3.up * .1f, Time.deltaTime * 10f);
            }
            else
            {

                Debug.Log("No ground detected below footTarget");
            }
        }
        else
        {
            footTarget.transform.position = lastStep;
        }

            //footTarget.transform.position = lastStep + Vector3.up * 1f; // Slightly above ground
        
    }

    bool CanStep(GameObject footTarget, Vector3 stepTarget)
    {
        Debug.DrawLine(footTarget.transform.position, stepTarget, Color.green);
        Debug.Log(Vector3.Distance(footTarget.transform.position, stepTarget));
        if (Vector3.Distance(footTarget.transform.position, stepTarget) > 3f)//1.9f)
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
