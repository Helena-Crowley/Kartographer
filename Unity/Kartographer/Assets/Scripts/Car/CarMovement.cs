using UnityEngine;
using UnityEngine.InputSystem;

/// \file CarMovement.cs
/// \brief Handles car movement using basic phyics and empty game objects.
/// \ingroup Vehicle

public class CarMovement : MonoBehaviour
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
    private PlayerControls controls;
    private InputAction driveAction;
    private Vector2 inputVector;

    [Header("Reset Car")]
    private InputAction resetAction;
    public Transform resetPoint;

    [Header("---TESTING----")]
    private Camera playerCamera;
    public Vector3 cameraOffset;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        driveAction = controls.Car.Drive;
        resetAction = controls.Car.Reset;
        resetAction.performed += _ => ResetCar();
        controls.Enable();
    }

    void OnDisable()
    {
        resetAction.performed -= _ => ResetCar();
        controls.Disable();
    }

    private void Start()
    {
        transform.position += Vector3.up * 0.5f;
        // DO NOT CHANGE SUSPENSION REST DIST FORMULA
        suspensionRestDistance = carTransform.position.y - frontLeftTireMesh.position.y + tireRadius;
        springStrength = springStrength * 10000;
        springDamper = springDamper * 100;
    }

    private void Update()
    {
        CheckUserInput();
    }

    private void FixedUpdate()
    {
        PhysicsUpdateTire(frontLeftTire, frontLeftTireMesh, carRigidBody);
        PhysicsUpdateTire(frontRightTire, frontRightTireMesh, carRigidBody);
        PhysicsUpdateTire(rearLeftTire, rearLeftTireMesh, carRigidBody);
        PhysicsUpdateTire(rearRightTire, rearRightTireMesh, carRigidBody);
    }

    /// <summary>
    /// Handles user input, calls respective functions/adjusts needed variables.
    ///     Utilizes Input Action system.
    /// </summary>
    /// <returns>None</returns>
    private void CheckUserInput()
    {
        //x is x and y is z (vector 2 to 3)
        inputVector = driveAction.ReadValue<Vector2>();
        // forward control
        if (inputVector.y > 0)
        {
            accelInput = appliedAcceleration;
        }
        // backward control
        else if (inputVector.y < 0)
        {
            accelInput = -appliedAcceleration;
        }
        else
        {
            accelInput = 0f;
        }
        if (inputVector.x > 0 || inputVector.x < 0)
        {
            turnDirection = inputVector.x;
        }
        else
        {
            turnDirection = 0f;
        }

        //turning
        if (Mathf.Abs(turnDirection) > 0.01f)
        {
            ApplySteering(turnDirection, true);
        }
        else
        {
            ApplySteering(turnDirection);
        }
    }

    /// <summary>
    /// Resets car's position when stuck, uses current position and adds .5f to y-axis.
    /// </summary>
    /// <returns>None</returns>
    private void ResetCar()
    {
        Vector3 raisedPosition = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);

        carRigidBody.linearVelocity = Vector3.zero;
        carRigidBody.angularVelocity = Vector3.zero;

        transform.position = raisedPosition;
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        currentYRotation = 0f;
        accelInput = 0f;

        Debug.Log("Car Reset: Lifted above current position.");
    }

    /// <summary>
    /// Applies all physics updates to the tire empties and the tire mesh emptys. Uses Raycast to detect
    ///     distance from ground for suspension calculation. Uses FixedUpdate() to run physics simulations.
    /// </summary>
    /// <param name="tireTransform"> tire empty transform to apply physics at it's given point </param>
    /// <param name="tireMesh"> Transform of the tireMesh object </param>
    /// <param name="carRigidBody"> gameObject's rigidbody component </param>
    /// <returns>None</returns>
    private void PhysicsUpdateTire(Transform tireTransform, Transform tireMesh, Rigidbody carRigidBody)
    {
        RaycastHit tireHit;
        Vector3 rayStart = tireMesh.position;
        Ray tireRay = new Ray(rayStart, -tireTransform.up);
        float rayLength = tireRadius + suspensionRestDistance;

        if (Physics.Raycast(tireRay, out tireHit, rayLength, layerGround))
        {
            float carSpeed = Vector3.Dot(carTransform.forward, carRigidBody.linearVelocity);

            ApplySuspension(tireTransform, tireHit);
            ApplyAcceleration(tireTransform, carSpeed);
            ApplyTireGrip(tireTransform, carSpeed);
            VisualUpdateTire(tireMesh);
        }
        else
        {
            // adjust gravity muliplier to adjust floatiness of car!!!
            carRigidBody.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// Updates tire mesh to turn, updates visual mesh and uses collisions to turn vehicle
    /// </summary>
    /// <param name="tireMesh"> Transform of the tireMesh object </param>
    /// <returns>None</returns>
    private void VisualUpdateTire(Transform tireMesh)
    {
        float tireRadius = 0.35f;
        Vector3 localVelocity = carTransform.InverseTransformDirection(carRigidBody.linearVelocity);
        float forwardSpeed = localVelocity.z;

        float rotationSpeed = forwardSpeed / (2 * Mathf.PI * tireRadius);
        float rotationAngle = -rotationSpeed * 360f * Time.fixedDeltaTime;

        tireMesh.Rotate(Vector3.right, rotationAngle, Space.Self);
    }

    /// <summary>
    /// Applies suspension force along spring z-axis
    /// </summary>
    /// <param name="tireTransform"> tire empty transform to apply physics at it's given point </param>
    /// <param name="tireHit"> returns bool if RayCast hit </param>
    /// <returns>None</returns>
    private void ApplySuspension(Transform tireTransform, RaycastHit tireHit)
    {
        Vector3 springDirection = tireTransform.up;
        Vector3 tireWorldVelocity = GetWorldVelocity(carRigidBody, tireTransform);

        float offset = suspensionRestDistance - tireHit.distance;
        float velocityOnSpring = Vector3.Dot(springDirection, tireWorldVelocity);
        float suspensionForce = (offset * springStrength) - (velocityOnSpring * springDamper);

        carRigidBody.AddForceAtPosition(springDirection * suspensionForce, tireTransform.position);
    }

    /// <summary>
    /// Applies force for accelerating, applies engine-breaking when no input is detected
    /// </summary>
    /// <param name="tireTransform"> tire empty transform to apply physics at it's given point </param>
    /// <param name="carSpeed"> Current car world speed </param>
    /// <returns>None</returns>
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

    /// <summary>
    /// Applies rotation to tire mesh turning the car (non-physics)
    /// </summary>
    /// <param name="turnDirection"> Direction wheels are going to point </param>
    /// <param name="isTurning"> User input toggles bool when turn key pressed</param>
    /// <returns>None</returns>
    private void ApplyTireGrip(Transform tireTransform, float carSpeed)
    {
        Vector3 steeringDir = tireTransform.right;
        float horizontalVelocity = Vector3.Dot(tireTransform.right, GetWorldVelocity(carRigidBody, tireTransform));

        EmitParticles(horizontalVelocity, carSpeed);

        float desiredVelocityChange = -horizontalVelocity * tireGripFactor01;

        float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;
        carRigidBody.AddForceAtPosition(steeringDir * tireMass * desiredAcceleration, tireTransform.position);
    }

    /// <summary>
    /// Emits particles given carSpeed threshold and userinput
    /// ----emits when initally accelerating
    /// ----emits when sliding
    /// </summary>
    /// <param name="steeringVelocity"> Speed car is sliding (loss of traction) </param>
    /// <param name="carSpeed"> Current car world speed </param>
    /// <returns>None</returns>
    private void EmitParticles(float steeringVelocity, float carSpeed)
    {
        bool shouldEmit = false;

        if (Mathf.Abs(accelInput) > 0.0f && carSpeed < 2.5f)
        {
            shouldEmit = true;
        }
        if (Mathf.Abs(steeringVelocity) > slideThreshold)
        {
            shouldEmit = true;
        }

        var emission1 = particles1.emission;
        var emission2 = particles2.emission;
        emission1.enabled = shouldEmit;
        emission2.enabled = shouldEmit;
    }

    /// <summary>
    /// Applies rotation to tire mesh turning the car (non-physics)
    /// </summary>
    /// <param name="turnDirection"> Direction wheels are going to point </param>
    /// <param name="isTurning"> User input toggles bool when turn key pressed</param>
    /// <returns>None</returns>
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

    /// <summary>
    /// Returns velocity at a given point with respect to world position.
    /// </summary>
    /// <param name="rb">Rigidbody for GetPointVelocity().</param>
    /// <param name="objTransform">Transform for GetPointVelocity().</param>
    /// <returns>The Vector3 velocity.</returns>
    private Vector3 GetWorldVelocity(Rigidbody rb, Transform objTransform)
    {

        Vector3 velocity = rb.GetPointVelocity(objTransform.position);
        return velocity;
    }
}
