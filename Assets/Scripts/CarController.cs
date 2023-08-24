using System.Collections;
using TMPro;
using UnityEngine;
public enum GearState
{
    Neutral,
    Running,
    CheckingChange,
    Changing
}
/*
public enum ReverseState
{
    Reverse,
    ExitReverse,
    Forwards
}*/

public class CarController : MonoBehaviour
{
    [SerializeField] private CameraScript camScript;

    //Display
    [SerializeField] private TMP_Text rpmText;
    [SerializeField] private TMP_Text gearText;
    [SerializeField] private TMP_Text speedText;

    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    [SerializeField] private float RPM;
    [SerializeField] private float wheelkmh;
    [SerializeField] private float redline;
    [SerializeField] private float idleRPM;
    [SerializeField] private int currentGear = 0;


    // Transmission
    [SerializeField] private float[] gearRatios;
    [SerializeField] private float differentialRatio;
    [SerializeField] private float currentTorque;
    private float clutch; //das k�nnte sp�ter f�r den enterhaken wichtig werden
    private float wheelRPM;
    [SerializeField] AnimationCurve hpToRPMCurve;

    //gear changes
    [SerializeField] private GearState gearState;
    [SerializeField] private float increaseGearRPM;
    [SerializeField] private float decreaseGearRPM;
    [SerializeField] private float changeGearTime = 0.5f;

    //reverse stuff
    //[SerializeField] private ReverseState reverseState = ReverseState.Forwards;
    [SerializeField] private float slipAngle;

    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    //Check if the Car is on its Roof
    [SerializeField] private Transform carTransform;
    [SerializeField] private float kineticSpeed;
    private BoxCollider carCollider;
    private int currentlyColliding = 0;
    

    //Center of Mass
    private Rigidbody carRigidBody;
    [SerializeField] private float CenterOfMassYOffset = -0.4f;
    [SerializeField] private float CenterOfMassZOffset = 0.4f;


    public LapManager lapManager;
    public bool useLapManager = true; 
    private Vector3 startPosition;
    private Vector3 lastCheckpoint;
    private Vector3 lastCheckpointRotation;

    private void Start()
    {
        gearState = GearState.Running;
        carRigidBody = GetComponent<Rigidbody>();
        carRigidBody.centerOfMass = new Vector3(carRigidBody.centerOfMass.x, carRigidBody.centerOfMass.y + CenterOfMassYOffset, carRigidBody.centerOfMass.z + CenterOfMassZOffset);
        startPosition = transform.position;
        lastCheckpoint = transform.position; // Setze den letzten Checkpoint auf die Startposition
        carCollider = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// 1 : up a gear
    /// 2 : down a gear
    /// </summary>
    /// <param name="gearChange"></param>
    /// <returns></returns>


    IEnumerator ChangeGear(int gearChange)
    {
        gearState = GearState.CheckingChange;
        if (currentGear + gearChange >= 0)
        {
            if (gearChange > 0)
            {
                yield return new WaitForSeconds(0.7f);
                if (RPM < increaseGearRPM || currentGear >= gearRatios.Length - 1)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            if (gearChange < 0)
            {
                yield return new WaitForSeconds(0.1f);

                if (RPM > decreaseGearRPM || currentGear <= 0)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            gearState = GearState.Changing;
            yield return new WaitForSeconds(changeGearTime);
            currentGear += gearChange;
        }

        if (gearState != GearState.Neutral)
            gearState = GearState.Running;

    }

    private void OnCollisionStay(Collision collision)
    {
        CheckStuckOnRoof();
    }

    private void FixedUpdate()
    {
        kineticSpeed = carRigidBody.velocity.magnitude;
        
        CalcWheelKMH();
        DisplayValues();
        GetInput();
        HandleMovement();
        HandleSteering();
        UpdateWheels();
        
       
        
    }
    /// <summary>
    /// uses Diameter of the wheel collider to calc kmh and also the movement in each frame for kinetic speed
    /// </summary>
    private void CalcWheelKMH()
    {
        //Calc wheel speed
        wheelkmh = rearRightWheelCollider.rpm * 60 * rearRightWheelCollider.radius / 1000 * Mathf.PI;
    }

    private void DisplayValues()
    {
        rpmText.text = Mathf.RoundToInt(RPM) + "RPM";

        int gearForText = currentGear;
        gearText.text = gearForText.ToString();

        speedText.text = Mathf.RoundToInt(kineticSpeed) + "KMH";
    }

    private void GetInput()
    {
        if ((lapManager.carsActive == true || !useLapManager) && !lapManager.openMenu)
        {
            // Steering Input
            horizontalInput = Input.GetAxis("Horizontal");

            // Acceleration Input
            verticalInput = Input.GetAxis("Vertical");
            if(frontLeftWheelCollider.isGrounded&& frontRightWheelCollider.isGrounded)
            {
                camScript.CheckCameraBackward(true);
            } else
            {
                camScript.CheckCameraBackward(false);
            }
            if (gearState != GearState.Changing)
            {
                clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.fixedDeltaTime);
            }
            else
            {
                clutch = 0;
            }
        }
    }

    private float CalculateTorque(float gasInput)
    {
        float torque = 0;
        if (RPM < idleRPM + 200 && gasInput == 0 && currentGear == 0)
        {
            gearState = GearState.Neutral;
        }
        if (gearState == GearState.Running && clutch > 0)
        {
            if (RPM > increaseGearRPM)
            {
                StartCoroutine(ChangeGear(1));
            }
            else if (RPM < decreaseGearRPM)
            {
                StartCoroutine(ChangeGear(-1));
            }
        }
        
            if (clutch < 0.1f)
            {
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redline * gasInput) + Random.Range(-50, 50), Time.fixedDeltaTime);
            }
            else
            {
                wheelRPM = Mathf.Abs((rearRightWheelCollider.rpm + rearLeftWheelCollider.rpm) / 2f) * gearRatios[currentGear] * differentialRatio;
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.fixedDeltaTime * 3f);
                torque = (hpToRPMCurve.Evaluate(RPM / redline) * motorForce / RPM) * gearRatios[currentGear] * differentialRatio * 5252f * clutch;
            }
        
        return torque;
    }
    

    private void HandleMovement()
    {

        float gasInput = verticalInput;
        float brakeInput;
        float movingDirection = Vector3.Dot(transform.forward, carRigidBody.velocity);
        if (gearState != GearState.Changing)
        {
            if (gearState == GearState.Neutral)
            {
                clutch = 0;
                if (Mathf.Abs(gasInput) > 0) gearState = GearState.Running;
            }
            else
            {
                clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);
            }
        }
        else
        {
            clutch = 0;
        }
        if (movingDirection < -0.5f && gasInput > 0)
        {
            brakeInput = Mathf.Abs(gasInput);
            gasInput = 0;

        }
        else if (movingDirection > 0.5f && gasInput < 0)
        {

            brakeInput = Mathf.Abs(gasInput);
            gasInput = 0;
        }
        else
        {
            brakeInput = 0;
        }
        ApplyBraking(brakeInput);
        currentTorque = CalculateTorque(gasInput);
        ApplyTorque(gasInput);
    }

    private void ApplyBraking(float brakeInput)
    {
        currentbreakForce = brakeInput * breakForce;
        frontRightWheelCollider.brakeTorque = currentbreakForce * 0.7f;
        frontLeftWheelCollider.brakeTorque = currentbreakForce * 0.7f;
        rearLeftWheelCollider.brakeTorque = currentbreakForce * 0.3f;
        rearRightWheelCollider.brakeTorque = currentbreakForce * 0.3f;
    }

    private void ApplyTorque(float gasInput)
    {
        rearLeftWheelCollider.motorTorque = currentTorque * gasInput;
        rearRightWheelCollider.motorTorque = currentTorque * gasInput;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void CheckStuckOnRoof()
    {
        if (kineticSpeed < 1) //only when lying still!
        {
            if (carTransform.rotation.eulerAngles.x > 80f && carTransform.rotation.eulerAngles.x < 280f
            || carTransform.rotation.eulerAngles.z > 80f && carTransform.rotation.eulerAngles.z < 280f)
            {
                StartCoroutine(stuckOnRoofCoroutine());
            }
        }

    }

    private IEnumerator stuckOnRoofCoroutine()
    {
        //wait and if its still turned around, reset the car!
        if (carTransform.rotation.eulerAngles.x > 80f && carTransform.rotation.eulerAngles.x < 280f
            || carTransform.rotation.eulerAngles.z > 80f && carTransform.rotation.eulerAngles.z < 280f)
        {
            yield return new WaitForSeconds(1.5f);
            if (carTransform.rotation.eulerAngles.x > 80f && carTransform.rotation.eulerAngles.x < 280f
            || carTransform.rotation.eulerAngles.z > 80f && carTransform.rotation.eulerAngles.z < 280f)
            {
                carTransform.Rotate(new Vector3(-carTransform.rotation.eulerAngles.x, 0, -carTransform.rotation.eulerAngles.z));
            }
        }
        yield break;

    }
     public void ResetCar()
    {
        // Setze das Auto auf die Startposition zurück
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //rpm auf 1000 oder so
        //lenkung auf 0
        currentSteerAngle = 0;
        RPM = 1000;
    }
    public void ResetToLastCheckpoint(){
        // Setze das Auto auf den letzten Checkpoint zurück
        transform.position = lastCheckpoint;
        transform.rotation = Quaternion.Euler(lastCheckpointRotation); // Setze die Rotation auf (0, 0, 0)
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
     public void SetLastCheckpoint(Vector3 checkpoint, Vector3 checkpointRotation)
    {
        lastCheckpoint = checkpoint;
        lastCheckpointRotation = checkpointRotation;
    }
    }
