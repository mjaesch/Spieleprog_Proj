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
    [SerializeField] private CameraScript cameraScript;
    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;

    [TagField]
    [Tooltip("Obstacles with this tag will be ignored.  "
        + "It's a good idea to set this field to the player's tag")]
    public string IgnoreTag = string.Empty;

    public Transform grappleTip, cam, car;
    

    [Header("Grappling Behaviour")]
    [Tooltip("How far to raycast to place the aim target")]
    public float maxGrapDistance = 200;
    private SpringJoint joint;
    public float spring =8.5f;
    public float damper = 7f;
    public float massScale = 4.5f;

    [Header("Camera and Crosshair")]
    public RectTransform ReticleImage;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        } else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
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
        Debug.Log(" cam fwd " + cam.forward);
        Debug.Log(" xhair pos " + cameraScript.getCrosshairOffset().normalized);
        

        Vector3 directionOfGrapple = cam.forward + cameraScript.getCrosshairOffset().normalized;
        directionOfGrapple.Normalize();
        Debug.Log(" both " + directionOfGrapple);
        if (Physics.Raycast(cam.position, directionOfGrapple, out hit, maxGrapDistance, whatIsGrappleable))
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
    /// <summary>
    /// Quelle: cinemachine MoveAimTarget.cs
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="camPos"></param>
    /// <returns></returns>
    private Vector3 GetProjectedAimTarget(Vector3 pos, Vector3 camPos)
    {
        var origin = pos;
        var fwd = (pos - camPos).normalized;
        pos += maxGrapDistance * fwd;
        if (whatIsGrappleable != 0 && RaycastIgnoreTag(
            new Ray(origin, fwd),
            out RaycastHit hitInfo, maxGrapDistance, whatIsGrappleable))
        {
            pos = hitInfo.point;
        }
        return pos;
    }
    /// <summary>
    /// Quelle: cinemachine MoveAimTarget.cs
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="hitInfo"></param>
    /// <param name="rayLength"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    private bool RaycastIgnoreTag(
        Ray ray, out RaycastHit hitInfo, float rayLength, int layerMask)
    {
        const float PrecisionSlush = 0.001f;
        float extraDistance = 0;
        while (Physics.Raycast(
            ray, out hitInfo, rayLength, layerMask,
            QueryTriggerInteraction.Ignore))
        {
            if (IgnoreTag.Length == 0 || !hitInfo.collider.CompareTag(IgnoreTag))
            {
                hitInfo.distance += extraDistance;
                return true;
            }

            // Ignore the hit.  Pull ray origin forward in front of obstacle
            Ray inverseRay = new Ray(ray.GetPoint(rayLength), -ray.direction);
            if (!hitInfo.collider.Raycast(inverseRay, out hitInfo, rayLength))
                break;
            float deltaExtraDistance = rayLength - (hitInfo.distance - PrecisionSlush);
            if (deltaExtraDistance < PrecisionSlush)
                break;
            extraDistance += deltaExtraDistance;
            rayLength = hitInfo.distance - PrecisionSlush;
            if (rayLength < PrecisionSlush)
                break;
            ray.origin = inverseRay.GetPoint(rayLength);
        }
        return false;
    }

}
