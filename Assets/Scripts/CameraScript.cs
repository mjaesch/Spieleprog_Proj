using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraScript : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public CinemachineComposer composer;
    public float xMin = -1.5f;
    public float xMax = 1.5f;
    public float yMin = 1f;
    public float yMax = 3f;
    public float sensitivity = 1;

    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        composer= vCam.GetCinemachineComponent<CinemachineComposer>();
        composer.m_TrackedObjectOffset.y = 0;
        composer.m_TrackedObjectOffset.x = 0;
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

        composer.m_TrackedObjectOffset.y += Input.GetAxis("Mouse Y") / 4;
        if(composer.m_TrackedObjectOffset.y < yMin)
        {
            composer.m_TrackedObjectOffset.y = yMin;
        }
        if (composer.m_TrackedObjectOffset.y > yMax)
        {
            composer.m_TrackedObjectOffset.y = yMax;
        }

        composer.m_TrackedObjectOffset.x += Input.GetAxis("Mouse X") / 4;
        if (composer.m_TrackedObjectOffset.x < xMin)
        {
            composer.m_TrackedObjectOffset.x = xMin;
        }
        if (composer.m_TrackedObjectOffset.x > xMax)
        {
            composer.m_TrackedObjectOffset.x = xMax;
        }
    }
}
