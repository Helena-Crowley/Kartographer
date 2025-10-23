using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

/// \file CarMovement.cs
/// \brief Handles car movement using basic phyics and empty game objects.
/// \ingroup Vehicle

public class CarMovement : NetworkBehaviour
{
    [Header("Tire Object")]
    public Transform frontLeftTire;
    public Transform frontRightTire;
    public Transform rearLeftTire;
    public Transform rearRightTire;

    [Header("Tire Visuals")]
    public Transform frontLeftTireMesh;
    public Transform frontRightTireMesh;
    public Transform rearLeftTireMesh;
    public Transform rearRightTireMesh;

    [Header("Tire Particles")]
    public ParticleSystem particles1;
    public ParticleSystem particles2;
    public float slideThreshold;

    [Header("Car")]
    public Rigidbody carRigidBody;
    public Transform carTransform;

    [Header("Misc")]
    public LayerMask layerGround;
    private float normalizedSpeed;
    public float tireRadius;

    [Header("Suspension")]
    public float springStrength;
    public float springDamper;
    private float suspensionRestDistance;
    public float gravityMultiplier;

    [Header("Steering")]
    public float tireMass;
    public float tireGripFactor01;
    public AnimationCurve tireRotationSpeed;
    private float currentYRotation = 0f;
    private float turnDirection = 0f;
    public float maxRotation = 45f;

    [Header("Acceleration/Breaking")]
    public AnimationCurve powerCurve;
    public float appliedAcceleration;
    public float carMaxSpeed;
    private float accelInput;
    public float engineBrakingStrength = 300f;

    [Header("Input Systems")]
    //private PlayerControls controls;
    //private InputAction driveAction;
    [SerializeField] private InputActionReference driveAction;
    private Vector2 inputVector;

    [Header("Reset Car")]
    [SerializeField] private InputActionReference resetAction;
    public Transform resetPoint;

    [Header("---TESTING----")]
    private Camera playerCamera;
    public Vector3 cameraOffset;

    [Header("Audio")]
    private CartAudio cartAudio;
    private WindAudio windAudio;
    public AudioClip slidingSoundEffect;
    private AudioSource skidSource = null;
    private bool wasMoving = false;

    private bool shouldEmit;

    private Vector3 lastPosition;
    private CartStats cartStats;
    [HideInInspector]
    public float totaldistance = 0f;
    public bool isCharged = true;
    private PlayerInputManager inputManager;
    public GameObject driverSeat;

    private void Start()
    {
        transform.position += Vector3.up * 0.5f;
        lastPosition = transform.position;
        // DO NOT CHANGE SUSPENSION REST DIST FORMULA
        suspensionRestDistance = carTransform.position.y - frontLeftTireMesh.position.y + tireRadius;
        springStrength = springStrength * 10000;
        springDamper = springDamper * 100;
        inputManager = driverSeat.GetComponentInChildren<PlayerInputManager>();

        cartStats = GetComponent<CartStats>();

        cartAudio = GetComponent<CartAudio>();
        windAudio = GetComponent<WindAudio>();

        Physics.queriesHitBackfaces = true;
    }

    private void Update()
    {

        if (isCharged)
        {
            Vector2 userInput = driveAction.action.ReadValue<Vector2>();
            // SendInputServerRpc(userInput);
        }

        if (resetAction.action.WasPressedThisFrame()) ResetCarServerRpc();

        else
        {
            accelInput = 0f;
            turnDirection = 0f;
            ApplySteering(turnDirection);
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        CheckUserInput();

        PhysicsUpdateTire(frontLeftTire, frontLeftTireMesh, carRigidBody);
        PhysicsUpdateTire(frontRightTire, frontRightTireMesh, carRigidBody);
        PhysicsUpdateTire(rearLeftTire, rearLeftTireMesh, carRigidBody);
        PhysicsUpdateTire(rearRightTire, rearRightTireMesh, carRigidBody);

        if (inputManager != null)
        {
            if (inputManager.InCart)
            {
                cartStats.batteryCanvas.SetActive(true);
                cartStats.distance = DistanceTravelled();
                cartStats.TakeDamage();
            }
            else
            {
                cartStats.batteryCanvas.SetActive(false);
            }
        }

    }

    public float DistanceTravelled()
    {
        float distanceTravelled = Vector3.Distance(lastPosition, transform.position);
        totaldistance += distanceTravelled;

        lastPosition = transform.position;
        return totaldistance;
    }

    private bool CheckUserInput()
    {
        inputVector = driveAction.action.ReadValue<Vector2>();

        bool hasInput = false;

        // Forward/backward
        if (inputVector.y > 0)
        {
            accelInput = appliedAcceleration;
            hasInput = true;
        }
        else if (inputVector.y < 0)
        {
            accelInput = -appliedAcceleration;
            hasInput = true;

        }
        else
        {
            accelInput = 0f;
        }

        // Left/right
        if (inputVector.x != 0)
        {
            turnDirection = inputVector.x;
        }
        else
        {
            turnDirection = 0f;

        }

        cartAudio.PlaySpeedDependentSound(normalizedSpeed);
        windAudio.UpdateWind(normalizedSpeed);


        // detect transitions
        if (hasInput && !wasMoving)
            cartAudio.PlayTickSound();
        if (!hasInput && wasMoving)
            cartAudio.PlayTickSound();


        // update state
        wasMoving = hasInput;


        return hasInput;
    }


    [ServerRpc(RequireOwnership = false)]
    private void ResetCarServerRpc()
    {
        Vector3 raisedPosition = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);

        carRigidBody.linearVelocity = Vector3.zero;
        carRigidBody.angularVelocity = Vector3.zero;

        transform.position = raisedPosition;
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        currentYRotation = 0f;
        accelInput = 0f;
        cartStats.health = 100;

        Debug.Log("Car Reset: Lifted above current position.");
    }

    // [ServerRpc(RequireOwnership = false)]
    // private void SendInputServerRpc(Vector2 input)
    // {
    //     inputVector = input;

    // }

    private void PhysicsUpdateTire(Transform tireTransform, Transform tireMesh, Rigidbody carRigidBody)
    {
        Vector3 rayStart = tireMesh.position;
        float rayLength = tireRadius + suspensionRestDistance;

        // --- OverlapSphere correction ---
        float correctionRadius = tireRadius * 0.5f;
        Collider[] overlaps = Physics.OverlapSphere(rayStart, correctionRadius, layerGround);

        if (overlaps.Length > 0)
        {
            foreach (var col in overlaps)
            {
                Vector3 closest;
                RaycastHit hit;
                if (Physics.Raycast(rayStart + Vector3.up, Vector3.down, out hit, 2f * correctionRadius, layerGround))
                {
                    closest = hit.point;
                }
                else
                {
                    // fallback — just nudge tire up a tiny bit
                    closest = rayStart + tireTransform.up * 0.01f;
                }

                float fakeDistance = Vector3.Distance(rayStart, closest);

                // Apply suspension force using the overlap helper
                ApplySuspensionFromOverlap(tireTransform, closest, fakeDistance, col.transform.up, 1);

            }
        }

        if (Mathf.Abs(turnDirection) > 0.01f)
        {
            ApplySteering(turnDirection, true);
        }
        else
        {
            ApplySteering(turnDirection);
        }

        // --- Normal suspension raycasts ---
        RaycastHit hitDown = new RaycastHit();
        RaycastHit hitUp = new RaycastHit();
        bool hitGroundDown = Physics.Raycast(rayStart, -tireTransform.up, out hitDown, rayLength, layerGround);
        bool hitGroundUp = !hitGroundDown && Physics.Raycast(rayStart, tireTransform.up, out hitUp, rayLength, layerGround);

        if (hitGroundDown)
        {
            float carSpeed = Vector3.Dot(carTransform.forward, carRigidBody.linearVelocity);

            ApplySuspension(tireTransform, hitDown);
            ApplyAcceleration(tireTransform, carSpeed);
            ApplyTireGrip(tireTransform, carSpeed);
            VisualUpdateTire(tireMesh);
        }
        else if (hitGroundUp)
        {
            ApplySuspension(tireTransform, hitUp, 1);
        }
        else
        {
            carRigidBody.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }

        // Debug rays
        Debug.DrawLine(rayStart, rayStart + tireTransform.up * rayLength, Color.red);
        Debug.DrawLine(rayStart, rayStart - tireTransform.up * rayLength, Color.green);
    }

    private void VisualUpdateTire(Transform tireMesh)
    {
        float tireRadius = 0.35f;
        Vector3 localVelocity = carTransform.InverseTransformDirection(carRigidBody.linearVelocity);
        float forwardSpeed = localVelocity.z;

        float rotationSpeed = forwardSpeed / (2 * Mathf.PI * tireRadius);
        float rotationAngle = -rotationSpeed * 360f * Time.fixedDeltaTime;

        tireMesh.Rotate(Vector3.right, rotationAngle, Space.Self);
    }

    private void ApplySuspension(Transform tireTransform, RaycastHit tireHit, int scale = 1)
    {
        Vector3 springDirection = tireTransform.up;
        Vector3 tireWorldVelocity = GetWorldVelocity(carRigidBody, tireTransform);

        float offset = suspensionRestDistance - tireHit.distance;
        float velocityOnSpring = Vector3.Dot(springDirection, tireWorldVelocity);
        float suspensionForce = (offset * springStrength) - (velocityOnSpring * springDamper);

        carRigidBody.AddForceAtPosition(springDirection * suspensionForce, tireTransform.position * scale);
    }

    private void ApplySuspensionFromOverlap(Transform tireTransform, Vector3 closest, float fakeDistance, Vector3 normal, int scale = 1)
    {
        Vector3 springDirection = tireTransform.up;
        Vector3 tireWorldVelocity = GetWorldVelocity(carRigidBody, tireTransform);

        float offset = suspensionRestDistance - fakeDistance;
        float velocityOnSpring = Vector3.Dot(springDirection, tireWorldVelocity);
        float suspensionForce = (offset * springStrength) - (velocityOnSpring * springDamper);

        carRigidBody.AddForceAtPosition(springDirection * suspensionForce, tireTransform.position * scale);
    }

    private void ApplyAcceleration(Transform tireTransform, float carSpeed)
    {
        Vector3 accelDir = tireTransform.forward;

        normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carMaxSpeed);

        if (Mathf.Abs(accelInput) > 0.0f)
        {
            float availableTorque = powerCurve.Evaluate(normalizedSpeed) * accelInput;
            carRigidBody.AddForceAtPosition(accelDir * availableTorque, tireTransform.position);
        }
        // engine brake if no input
        else if (Mathf.Abs(carSpeed) > 0.1f)
        {
            float brakingMultiplier = Mathf.Clamp01(Mathf.Abs(carSpeed) / carMaxSpeed);
            float brakingForce = -Mathf.Sign(carSpeed) * engineBrakingStrength * brakingMultiplier;
            carRigidBody.AddForceAtPosition(accelDir * brakingForce, tireTransform.position);
        }
    }

    private void ApplyTireGrip(Transform tireTransform, float carSpeed)
    {
        Vector3 steeringDir = tireTransform.right;
        float horizontalVelocity = Vector3.Dot(tireTransform.right, GetWorldVelocity(carRigidBody, tireTransform));

        EmitParticles(horizontalVelocity, carSpeed);

        float desiredVelocityChange = -horizontalVelocity * tireGripFactor01;

        float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;
        carRigidBody.AddForceAtPosition(steeringDir * tireMass * desiredAcceleration, tireTransform.position);
    }

    private void EmitParticles(float steeringVelocity, float carSpeed)
    {
        // Determine if sliding based on particles or input
        shouldEmit = (Mathf.Abs(accelInput) > 0f && carSpeed < 2.5f) || Mathf.Abs(steeringVelocity) > slideThreshold;

        var emission1 = particles1.emission;
        var emission2 = particles2.emission;
        emission1.enabled = shouldEmit;
        emission2.enabled = shouldEmit;

        bool anyEmitting = particles1.particleCount > 0 || particles2.particleCount > 0;


        if (anyEmitting)
        {
            if (skidSource == null)
                skidSource = SoundManager.Instance.PlayLoopingSound(slidingSoundEffect, transform.position, 0.75f);
            else if (!skidSource.isPlaying)
                skidSource.Play();
            skidSource.transform.position = transform.position;
        }
        else
        {
            if (skidSource != null)
            {
                skidSource.volume = Mathf.Lerp(skidSource.volume, 0f, Time.fixedDeltaTime * 10f);
                if (skidSource.volume < 0.01f)
                {
                    SoundManager.Instance.StopSound(skidSource);
                    skidSource = null;
                }
            }
        }
    }


    private void ApplySteering(float turnDirection, bool isTurning = false)
    {
        float steeringSpeed = 200f * tireRotationSpeed.Evaluate(normalizedSpeed);
        float rotationDelta;

        if (isTurning)
        {
            rotationDelta = turnDirection * steeringSpeed * Time.deltaTime;
            currentYRotation += rotationDelta;

            currentYRotation = Mathf.Clamp(currentYRotation, -maxRotation, maxRotation);
        }
        else
        {
            // return to 0 steering angle
            currentYRotation = Mathf.MoveTowards(currentYRotation, 0f, 2.25f * steeringSpeed * Time.deltaTime);
        }

        Quaternion steerRotation = Quaternion.Euler(0f, currentYRotation, 0f);
        frontLeftTire.localRotation = steerRotation;
        frontRightTire.localRotation = steerRotation;
    }

    private Vector3 GetWorldVelocity(Rigidbody rb, Transform objTransform)
    {

        Vector3 velocity = rb.GetPointVelocity(objTransform.position);
        return velocity;
    }
}
