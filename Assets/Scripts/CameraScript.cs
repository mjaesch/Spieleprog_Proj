using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    private CinemachineComposer composer;
    private CinemachineTransposer transposer;
    [SerializeField] private Rigidbody carRigidbody;

    private bool cameraFacesBack;

    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        composer= vCam.GetCinemachineComponent<CinemachineComposer>();
        transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        cameraFacesBack = false;
    }

    
    void Update()
    {
        Debug.Log(Vector3.Angle(carRigidbody.velocity, carRigidbody.transform.forward));
        //Bedingungen player presses back + bewegung in richtung zurück, also speed größer als 1 und richtung zwischen 120 und 181 grad
        //es ist besser wenn ich das zusammen mit car controller mache
    }
    /// <summary>
    /// wird von CarController Aufgerufen wenn es einen vertical axis input unter 0 gibt!
    /// 
    /// Problem: wenn grappled wird, dann wird ständig gewechselt
    /// check for grounded?
    /// </summary>
    public void CheckCameraBackward(bool isGrounded)
    {
        float angleOfMovement = Vector3.Angle(carRigidbody.velocity, carRigidbody.transform.forward);
        if (isGrounded && carRigidbody.velocity.sqrMagnitude > 9 && (angleOfMovement>118f &&  angleOfMovement < 182f)){
            transposer.m_FollowOffset.z = 4;
            cameraFacesBack = true;
        }
        else
        {
            transposer.m_FollowOffset.z = -6;
            cameraFacesBack = false;
        }
    }

    public bool getCameraFacesBack() {
        return cameraFacesBack;
    }
}
