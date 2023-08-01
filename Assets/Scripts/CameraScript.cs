using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    [SerializeField]private CinemachineVirtualCamera vCam;
    [SerializeField]private CinemachineComposer composer;
    [SerializeField]private float xMin = -1.5f;
    [SerializeField]private float xMax = 1.5f;
    [SerializeField]private float yMin = 1f;
    [SerializeField]private float yMax = 3f;
    [SerializeField]private float sensitivity = 1;
    
    [SerializeField]private Image crosshair;
    
    [SerializeField] private float crosshairMovementMultiplier = 20;
    
    private Vector3 initialPos;
    private Vector3 newPos;
    [SerializeField]private float maxCrosshairX = 1f;
    [SerializeField]private float minCrosshairX = -1f;
    [SerializeField]private float maxCrosshairY = 1f;
    [SerializeField]private float minCrosshairY = -1f;

    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        composer= vCam.GetCinemachineComponent<CinemachineComposer>();
        //composer.m_TrackedObjectOffset.y = 0;
       // composer.m_TrackedObjectOffset.x = 0;

        initialPos = crosshair.rectTransform.position;
        newPos = crosshair.rectTransform.position;

        maxCrosshairX += initialPos.x;
        minCrosshairX += initialPos.x;
        maxCrosshairY += initialPos.y;
        minCrosshairY += initialPos.y;

    }

    // Update is called once per frame
    // TODO:
    // runtime änderung des trackedobjectoffset - done
    // get mouseInput -
    // give it bounds
    // give it a crosshair
    // damping?
    void Update()
    {
        
        /*
        composer.m_TrackedObjectOffset.y += Input.GetAxis("Mouse Y") / 6;
        if(composer.m_TrackedObjectOffset.y < yMin)
        {
            composer.m_TrackedObjectOffset.y = yMin;
        }
        if (composer.m_TrackedObjectOffset.y > yMax)
        {
            composer.m_TrackedObjectOffset.y = yMax;
        }

        composer.m_TrackedObjectOffset.x += Input.GetAxis("Mouse X") / 6;
        
        if (composer.m_TrackedObjectOffset.x < xMin)
        {
            composer.m_TrackedObjectOffset.x = xMin;
        }
        if (composer.m_TrackedObjectOffset.x > xMax)
        {
            composer.m_TrackedObjectOffset.x = xMax;
        }*/


        //TODO: die bounds ändern
        //newPos.x += composer.m_TrackedObjectOffset.x * crosshairMovementMultiplier;
        //newPos.y += composer.m_TrackedObjectOffset.y * crosshairMovementMultiplier;
        newPos.x += Input.GetAxis("Mouse X") * crosshairMovementMultiplier;
        if( newPos.x > maxCrosshairX)
        {
            newPos.x = maxCrosshairX;
        } else if (newPos.x < minCrosshairX)
        {
            newPos.x = minCrosshairX;
        }
        newPos.y += Input.GetAxis("Mouse Y") * crosshairMovementMultiplier;
        if (newPos.y > maxCrosshairY)
        {
            newPos.y = maxCrosshairY;
        }
        else if (newPos.y < minCrosshairY)
        {
            newPos.y = minCrosshairY;
        }
        crosshair.rectTransform.position = newPos;
    }

    public Vector3 getCrosshairOffset()
    {
        return newPos-initialPos;
    }
}
