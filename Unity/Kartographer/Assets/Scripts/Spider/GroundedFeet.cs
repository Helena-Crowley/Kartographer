
// // using System.Linq;
// // using UnityEngine;

// // [RequireComponent(typeof(Rigidbody))]
// // public class GroundedFeetPhysicsReactive : MonoBehaviour
// // {
// //     [System.Serializable]
// //     public class Foot
// //     {
// //         public GameObject footTarget;
// //         public Vector3 bodyOffset;

// //         [HideInInspector] public Vector3 plantedPos;
// //         [HideInInspector] public float stepDistanceOffset = 0f;
// //         [HideInInspector] public bool isStepping;
// //     }

// //     [Header("Feet Settings")]
// //     public Foot[] feet;
// //     public LayerMask groundLayer;
// //     public float stepDistance = 2f;
// //     public float stepSpeed = 5f;
// //     public float bodyOffset = 1.5f;

// //     [Header("Physics Settings")]
// //     public float mass = 5f;
// //     public float pushDamping = 0.9f; // how quickly stagger slows down
// //     public float staggerMultiplier = 1f; // how strongly it reacts to hits

// //     private Rigidbody rb;
// //     private bool groupATurn = true;

// //     private Vector3 staggerVelocity = Vector3.zero;

// //     void Start()
// //     {
// //         rb = GetComponent<Rigidbody>();
// //         rb.mass = mass;
// //         rb.interpolation = RigidbodyInterpolation.Interpolate;
// //         rb.freezeRotation = true; // keep upright
// //         rb.useGravity = false;    // disable gravity, vertical controlled by feet

// //         foreach (var foot in feet)
// //         {
// //             foot.plantedPos = StepTargetCast(foot.bodyOffset);
// //             foot.footTarget.transform.position = foot.plantedPos;

// //             // simple front/back step offset for alternating stepping
// //             foot.stepDistanceOffset = (foot == feet[0] || foot == feet[2]) ? stepDistance * 0.5f : 0f;
// //         }
// //     }

// //     void FixedUpdate()
// //     {
// //         // --- Apply stagger velocity and dampen it ---
// //         Vector3 horizontalVel = new Vector3(staggerVelocity.x, 0f, staggerVelocity.z);
// //         horizontalVel *= pushDamping;
// //         staggerVelocity = horizontalVel; // decay stagger over time

// //         rb.linearVelocity = new Vector3(horizontalVel.x, rb.linearVelocity.y, horizontalVel.z);

// //         // --- Feet stepping ---
// //         foreach (var foot in feet)
// //         {
// //             Vector3 stepTarget = StepTargetCast(foot.bodyOffset);

// //             if (!foot.isStepping)
// //             {
// //                 float dist = Vector3.Distance(foot.plantedPos, stepTarget);
// //                 if (dist > stepDistance + foot.stepDistanceOffset)
// //                 {
// //                     if (groupATurn)
// //                         StartCoroutine(StepFoot(foot, stepTarget));
// //                     groupATurn = !groupATurn;
// //                 }
// //                 else
// //                 {
// //                     foot.footTarget.transform.position = foot.plantedPos;
// //                 }
// //             }
// //         }

// //         // --- Vertical body adjustment to follow feet ---
// //         float avgFeetY = feet.Average(f => f.footTarget.transform.position.y);
// //         float targetY = avgFeetY + bodyOffset;

// //         rb.MovePosition(new Vector3(rb.position.x, targetY, rb.position.z));
// //     }

// //     System.Collections.IEnumerator StepFoot(Foot foot, Vector3 newPos)
// //     {
// //         foot.isStepping = true;
// //         Vector3 startPos = foot.plantedPos;
// //         float t = 0f;

// //         while (t < 1f)
// //         {
// //             t += Time.deltaTime * stepSpeed;
// //             Vector3 footPos = Vector3.Lerp(startPos, newPos, t);
// //             footPos.y += Mathf.Sin(t * Mathf.PI) * 0.2f; // lift foot
// //             foot.footTarget.transform.position = footPos;
// //             yield return null;
// //         }

// //         foot.plantedPos = newPos;
// //         foot.isStepping = false;
// //     }

// //     Vector3 StepTargetCast(Vector3 offset)
// //     {
// //         RaycastHit hit;
// //         Vector3 origin = transform.position + transform.TransformDirection(offset);

// //         if (Physics.Raycast(origin + Vector3.up * 4f, Vector3.down, out hit, 10f, groundLayer))
// //         {
// //             Debug.DrawRay(origin, Vector3.down * hit.distance, Color.blue);
// //             return hit.point;
// //         }

// //         return origin; // fallback
// //     }

// //     void OnCollisionEnter(Collision collision)
// //     {
// //         rb.AddForce(collision.impulse * staggerMultiplier, ForceMode.Impulse);
// //     }
// // }

// using System.Linq;
// using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
// public class GroundedFeetPhysicsReactive : MonoBehaviour
// {
//     [System.Serializable]
//     public class Foot
//     {
//         public GameObject footTarget;
//         public Vector3 bodyOffset;

//         [HideInInspector] public Vector3 plantedPos;
//         [HideInInspector] public float stepDistanceOffset = 0f;
//         [HideInInspector] public bool isStepping;
//     }

//     [Header("Feet Settings")]
//     public Foot[] feet;
//     public LayerMask groundLayer;
//     public float stepDistance = 2f;
//     public float stepSpeed = 5f;
//     public float bodyOffset = 1.5f;

//     [Header("Physics Settings")]
//     public float mass = 5f;
//     [Range(0f,1f)] public float pushDamping = 0.9f; // how quickly stagger slows down
//     public float staggerMultiplier = 1f; // strength of reaction to hits

//     private Rigidbody rb;
//     private bool groupATurn = true;

//     private Vector3 staggerVelocity = Vector3.zero;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//         rb.mass = mass;
//         rb.interpolation = RigidbodyInterpolation.Interpolate;
//         rb.freezeRotation = true; // keep upright
//         rb.useGravity = false;    // vertical controlled by feet
//         rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

//         foreach (var foot in feet)
//         {
//             foot.plantedPos = StepTargetCast(foot.bodyOffset);
//             foot.footTarget.transform.position = foot.plantedPos;

//             foot.stepDistanceOffset = (foot == feet[0] || foot == feet[2]) ? stepDistance * 0.5f : 0f;
//         }
//     }

//     void FixedUpdate()
//     {
//         // --- Apply horizontal stagger velocity ---
//         staggerVelocity *= pushDamping; // decay over time
//         rb.linearVelocity = new Vector3(staggerVelocity.x, rb.linearVelocity.y, staggerVelocity.z);

//         // --- Feet stepping ---
//         foreach (var foot in feet)
//         {
//             Vector3 stepTarget = StepTargetCast(foot.bodyOffset);

//             if (!foot.isStepping)
//             {
//                 float dist = Vector3.Distance(foot.plantedPos, stepTarget);
//                 if (dist > stepDistance + foot.stepDistanceOffset)
//                 {
//                     if (groupATurn)
//                         StartCoroutine(StepFoot(foot, stepTarget));
//                     groupATurn = !groupATurn;
//                 }
//                 else
//                 {
//                     foot.footTarget.transform.position = foot.plantedPos;
//                 }
//             }
//         }

//         // --- Vertical body adjustment ---
//         float avgFeetY = feet.Average(f => f.footTarget.transform.position.y);
//         float targetY = avgFeetY + bodyOffset;

//         rb.MovePosition(new Vector3(rb.position.x, targetY, rb.position.z));
//     }

//     System.Collections.IEnumerator StepFoot(Foot foot, Vector3 newPos)
//     {
//         foot.isStepping = true;
//         Vector3 startPos = foot.plantedPos;
//         float t = 0f;

//         while (t < 1f)
//         {
//             t += Time.deltaTime * stepSpeed;
//             Vector3 footPos = Vector3.Lerp(startPos, newPos, t);
//             footPos.y += Mathf.Sin(t * Mathf.PI) * 0.2f; // lift foot
//             foot.footTarget.transform.position = footPos;
//             yield return null;
//         }

//         foot.plantedPos = newPos;
//         foot.isStepping = false;
//     }

//     Vector3 StepTargetCast(Vector3 offset)
//     {
//         RaycastHit hit;
//         Vector3 origin = transform.position + transform.TransformDirection(offset);

//         if (Physics.Raycast(origin + Vector3.up * 4f, Vector3.down, out hit, 10f, groundLayer))
//         {
//             Debug.DrawRay(origin, Vector3.down * hit.distance, Color.blue);
//             return hit.point;
//         }

//         return origin; // fallback
//     }

//     void OnCollisionEnter(Collision collision)
//     {
//         // Accumulate horizontal impulse into stagger velocity
//         Vector3 impulse = collision.impulse;
//         impulse.y = 0f; // ignore vertical
//         staggerVelocity += impulse * staggerMultiplier;
//     }
// }


// cast ray from toe to ground
// if hit 
// if other toe is grounded
// toe move towards target
// toe is grounded

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
            toePositions[i].position = Vector3.MoveTowards(toePositions[i].position, targetStepPosition[i], 0.1f);
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