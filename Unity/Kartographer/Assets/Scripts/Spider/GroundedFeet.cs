
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
//     public float pushDamping = 0.9f; // how quickly stagger slows down
//     public float staggerMultiplier = 1f; // how strongly it reacts to hits

//     private Rigidbody rb;
//     private bool groupATurn = true;

//     private Vector3 staggerVelocity = Vector3.zero;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//         rb.mass = mass;
//         rb.interpolation = RigidbodyInterpolation.Interpolate;
//         rb.freezeRotation = true; // keep upright
//         rb.useGravity = false;    // disable gravity, vertical controlled by feet

//         foreach (var foot in feet)
//         {
//             foot.plantedPos = StepTargetCast(foot.bodyOffset);
//             foot.footTarget.transform.position = foot.plantedPos;

//             // simple front/back step offset for alternating stepping
//             foot.stepDistanceOffset = (foot == feet[0] || foot == feet[2]) ? stepDistance * 0.5f : 0f;
//         }
//     }

//     void FixedUpdate()
//     {
//         // --- Apply stagger velocity and dampen it ---
//         Vector3 horizontalVel = new Vector3(staggerVelocity.x, 0f, staggerVelocity.z);
//         horizontalVel *= pushDamping;
//         staggerVelocity = horizontalVel; // decay stagger over time

//         rb.linearVelocity = new Vector3(horizontalVel.x, rb.linearVelocity.y, horizontalVel.z);

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

//         // --- Vertical body adjustment to follow feet ---
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
//         rb.AddForce(collision.impulse * staggerMultiplier, ForceMode.Impulse);
//     }
// }

using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GroundedFeetPhysicsReactive : MonoBehaviour
{
    [System.Serializable]
    public class Foot
    {
        public GameObject footTarget;
        public Vector3 bodyOffset;

        [HideInInspector] public Vector3 plantedPos;
        [HideInInspector] public float stepDistanceOffset = 0f;
        [HideInInspector] public bool isStepping;
    }

    [Header("Feet Settings")]
    public Foot[] feet;
    public LayerMask groundLayer;
    public float stepDistance = 2f;
    public float stepSpeed = 5f;
    public float bodyOffset = 1.5f;

    [Header("Physics Settings")]
    public float mass = 5f;
    [Range(0f,1f)] public float pushDamping = 0.9f; // how quickly stagger slows down
    public float staggerMultiplier = 1f; // strength of reaction to hits

    private Rigidbody rb;
    private bool groupATurn = true;

    private Vector3 staggerVelocity = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true; // keep upright
        rb.useGravity = false;    // vertical controlled by feet
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        foreach (var foot in feet)
        {
            foot.plantedPos = StepTargetCast(foot.bodyOffset);
            foot.footTarget.transform.position = foot.plantedPos;

            foot.stepDistanceOffset = (foot == feet[0] || foot == feet[2]) ? stepDistance * 0.5f : 0f;
        }
    }

    void FixedUpdate()
    {
        // --- Apply horizontal stagger velocity ---
        staggerVelocity *= pushDamping; // decay over time
        rb.linearVelocity = new Vector3(staggerVelocity.x, rb.linearVelocity.y, staggerVelocity.z);

        // --- Feet stepping ---
        foreach (var foot in feet)
        {
            Vector3 stepTarget = StepTargetCast(foot.bodyOffset);

            if (!foot.isStepping)
            {
                float dist = Vector3.Distance(foot.plantedPos, stepTarget);
                if (dist > stepDistance + foot.stepDistanceOffset)
                {
                    if (groupATurn)
                        StartCoroutine(StepFoot(foot, stepTarget));
                    groupATurn = !groupATurn;
                }
                else
                {
                    foot.footTarget.transform.position = foot.plantedPos;
                }
            }
        }

        // --- Vertical body adjustment ---
        float avgFeetY = feet.Average(f => f.footTarget.transform.position.y);
        float targetY = avgFeetY + bodyOffset;

        rb.MovePosition(new Vector3(rb.position.x, targetY, rb.position.z));
    }

    System.Collections.IEnumerator StepFoot(Foot foot, Vector3 newPos)
    {
        foot.isStepping = true;
        Vector3 startPos = foot.plantedPos;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * stepSpeed;
            Vector3 footPos = Vector3.Lerp(startPos, newPos, t);
            footPos.y += Mathf.Sin(t * Mathf.PI) * 0.2f; // lift foot
            foot.footTarget.transform.position = footPos;
            yield return null;
        }

        foot.plantedPos = newPos;
        foot.isStepping = false;
    }

    Vector3 StepTargetCast(Vector3 offset)
    {
        RaycastHit hit;
        Vector3 origin = transform.position + transform.TransformDirection(offset);

        if (Physics.Raycast(origin + Vector3.up * 4f, Vector3.down, out hit, 10f, groundLayer))
        {
            Debug.DrawRay(origin, Vector3.down * hit.distance, Color.blue);
            return hit.point;
        }

        return origin; // fallback
    }

    void OnCollisionEnter(Collision collision)
    {
        // Accumulate horizontal impulse into stagger velocity
        Vector3 impulse = collision.impulse;
        impulse.y = 0f; // ignore vertical
        staggerVelocity += impulse * staggerMultiplier;
    }
}
