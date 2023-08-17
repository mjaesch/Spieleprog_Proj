using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
/// <summary>
/// Quellen: https://www.youtube.com/watch?v=Xgh4v1w5DxU - grappling behaviour
///          cinemachine dual target sample - custom reticle
/// </summary>
public class GrapplingController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform grappleTip, car, AimTarget;

    [Header("Grappling Behaviour")]
    [Tooltip("How far to raycast to place the aim target")]
    public float maxGrapDistance = 200;
    private SpringJoint joint;
    public float spring =8.5f;
    public float damper = 7f;
    public float massScale = 4.5f;

    public LapManager lapManager;
    public bool useLapManager = true;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lapManager.carsActive == true || !useLapManager)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartGrapple();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                StopGrapple();
            }
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        if (!joint) return;

        lineRenderer.SetPosition(0, grappleTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    private void StartGrapple()
    {
        RaycastHit hit;
       
        if (Physics.Raycast(transform.position, (AimTarget.position - transform.position), out hit, maxGrapDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = car.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(car.position,grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            lineRenderer.positionCount = 2;
        }
    }

    private void StopGrapple()
    {
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

}
