/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum GearState
{
    Neutral,
    Running,
    CheckingChange,
    Changing
}

public enum ReverseState
{
    Reverse,
    ExitReverse,
    Forwards
}

public class CarController : MonoBehaviour
{
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
    [SerializeField] private int currentGear = 1;
    

    // Transmission
    [SerializeField] private float[] gearRatios;
    [SerializeField] private float differentialRatio;
    private float currentTorque;
    private float clutch; //das k�nnte sp�ter f�r den enterhaken wichtig werden
    private float wheelRPM;
    [SerializeField] AnimationCurve hpToRPMCurve;

    //gear changes
    [SerializeField] private GearState gearState;
    [SerializeField] private float increaseGearRPM;
    [SerializeField] private float decreaseGearRPM;
    [SerializeField] private float changeGearTime = 0.5f;


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

    //Center of Mass
    private Rigidbody carRigidBody;
    [SerializeField] private float CenterOfMassYOffset = -0.4f;
    [SerializeField] private float CenterOfMassZOffset = 0.4f;

    private void Start()
    {
        gearState = GearState.Running;

        StartCoroutine(KinecticSpeedCalculator(0.5f));
        carRigidBody = GetComponent<Rigidbody>();
        carRigidBody.centerOfMass = new Vector3(carRigidBody.centerOfMass.x, carRigidBody.centerOfMass.y + CenterOfMassYOffset, carRigidBody.centerOfMass.z + CenterOfMassZOffset);
    }

    // Source : https://stackoverflow.com/questions/55042997/how-to-calculate-a-gameobjects-speed-in-unity
    private IEnumerator KinecticSpeedCalculator(float updateDelay)
    {
        YieldInstruction timedWait = new WaitForSeconds(updateDelay);
        Vector3 lastPosition = transform.position;
        float lastTimestamp = Time.time;

        while (enabled)
        {
            yield return timedWait;

            var deltaPosition = (transform.position - lastPosition).magnitude;
            var deltaTime = Time.time - lastTimestamp;

            if (Mathf.Approximately(deltaPosition, 0f)) // Clean up "near-zero" displacement
                deltaPosition = 0f;

            kineticSpeed = deltaPosition / deltaTime;


            lastPosition = transform.position;
            lastTimestamp = Time.time;
        }
    }

    IEnumerator ChangeGear(int gearChange)
    {
        gearState = GearState.CheckingChange;
        if(currentGear + gearChange >= 1 && currentGear + gearChange < gearRatios.Length - 1)
        {
            if (gearChange > 0)
            { 
                yield return new WaitForSeconds(0.7f);
                if ((currentGear != 0 && RPM < increaseGearRPM) || currentGear >= gearRatios.Length - 1)
                {
                    Debug.Log("exit Change gear");
                    gearState = GearState.Running;
                    yield break;
                }
            }
            if (gearChange < 0)
            {
                yield return new WaitForSeconds(0.1f);

                if (RPM > decreaseGearRPM || currentGear <= 1)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }

            gearState = GearState.Changing;
            yield return new WaitForSeconds(changeGearTime);
            currentGear += gearChange;
        } else if (gearChange == 100)
        {
            if(currentGear != 0)
            {
                gearState = GearState.Changing;
                yield return new WaitForSeconds(changeGearTime);
                currentGear = gearChange - 100;
            }
        }
        if (gearState != GearState.Neutral)
            gearState = GearState.Running;

    }

    private void FixedUpdate()
    {
        CalcWheelKMH();
        DisplayValues();
        GetInput();
        HandleMovement();
        HandleSteering();
        UpdateWheels();
        CheckStuckOnRoof();
    }
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

        speedText.text = Mathf.RoundToInt(wheelkmh) + "KMH";
    }

    private void GetInput()
    {
        // Steering Input
        horizontalInput = Input.GetAxis("Horizontal");

        // Acceleration Input
        verticalInput = Input.GetAxis("Vertical");
        if(gearState != GearState.Changing)
        {
            clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.fixedDeltaTime);
        } else
        {
            clutch = 0;
        }

        // Breaking Input
        //isBreaking = Input.GetKey(KeyCode.Space);
    }

    private float CalculateTorque(float gasInput)
    {
        float torque = 0;
        
        if(clutch < 0.1f)
        {
            RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redline * gasInput) + Random.Range(-50, 50), Time.fixedDeltaTime);
        }
        else
        {
            wheelRPM = Mathf.Abs(( rearRightWheelCollider.rpm + rearLeftWheelCollider.rpm ) / 2f) * gearRatios[currentGear] * differentialRatio;
            RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.fixedDeltaTime * 3f); //wieso 3f;
            torque = (hpToRPMCurve.Evaluate(RPM / redline) * gasInput / RPM) * gearRatios[currentGear] * differentialRatio * 5252f * clutch; //5252f ist die konversion zwischen hp und torque
        }
        return torque;
    }

    //idea: i want to attempt to shift into reverse, but then check if we are already in reverse
    private void HandleGearing(bool reverse)
    {
        if (gearState == GearState.Running && clutch > 0)
        {
            if (!reverse)
            {
                if (currentGear == 0)
                {
                    Debug.Log("start Coroutine case exit rev");
                    StartCoroutine(ChangeGear(1));
                }
                else if (RPM > increaseGearRPM)
                {
                    Debug.Log("start Coroutine case incr gear");
                    StartCoroutine(ChangeGear(1));
                }
                else if (RPM < decreaseGearRPM)
                {
                    Debug.Log("start Coroutine case decr gear");
                    StartCoroutine(ChangeGear(-1));
                }
            } else
            {
                Debug.Log("start Coroutine else case");
                StartCoroutine(ChangeGear(100));
            }
            
        }
    }

    private void HandleMovement()
    {

        float gasInput = 0;
        float brakeInput = 0;
        bool reverse = false;

        if (wheelkmh >= -1.5f && verticalInput > 0.0f) //stand still or moving fwd and hold fwd -> shift into a fwd gear and accel
        {
            //Debug.Log("case 1");
            gasInput = verticalInput;
        } else if (wheelkmh > 3.0f && verticalInput < 0.0f) //driving forward and holding back -> braking and also shift normally
        {

            //Debug.Log("case 2");
            brakeInput = verticalInput * -1;
        } else if (wheelkmh < 3.0f && verticalInput < 0.0f) //drive back and hold back -> shift into rev and accel
        {
            //Debug.Log("case 3");
            gasInput = verticalInput*-1;
            reverse = true;
        } else if(wheelkmh < -60.5f && verticalInput >= 0.0f) //drive back and hold fwd -> braking
        {
            //Debug.Log("case 4");
            brakeInput = verticalInput;
        }
        //gasInput = verticalInput > 0.0f ? verticalInput : 0;
        //brakeInput = verticalInput < 0.0f ? verticalInput * -1 : 0;

        HandleGearing(reverse);
        ApplyBreaking(brakeInput);
        ApplyTorque(CalculateTorque(gasInput));
    }

    private void ApplyBreaking(float brakeInput)
    {
        currentbreakForce = brakeInput * breakForce ;
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void ApplyTorque(float torque)
    {
        currentTorque = torque;
        rearLeftWheelCollider.motorTorque = currentTorque * motorForce;
        rearRightWheelCollider.motorTorque = currentTorque * motorForce;
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
        if(kineticSpeed < 1) //only when lying still!
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
            || carTransform.rotation.eulerAngles.z >80f && carTransform.rotation.eulerAngles.z < 280f)
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
}
*/