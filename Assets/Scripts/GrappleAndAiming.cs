using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
/// <summary>
/// Quellen: https://www.youtube.com/watch?v=Xgh4v1w5DxU - grappling behaviour
///          cinemachine dual target sample - custom reticle
/// </summary>
public class GrappleAndAiming : MonoBehaviour
{
    [Header("Aiming")]
    public Transform CarTransform;

    public CinemachineBrain Brain;
    public RectTransform ReticleImage;

    [Tooltip("How far to raycast to place the aim target")]
    public float AimDistance;

    [Tooltip("Objects on these layers will be detected")]
    public LayerMask CollideAgainst;

    [TagField]
    [Tooltip("Obstacles with this tag will be ignored.  "
        + "It's a good idea to set this field to the player's tag")]
    public string IgnoreTag = string.Empty;

    [Tooltip("The Vertical axis.  Value is -90..90. Controls the vertical orientation")]
    [AxisStateProperty]
    public AxisState VerticalAxis;

    /// <summary>The Horizontal axis.  Value is -180..180.  Controls the horizontal orientation</summary>
    [Tooltip("The Horizontal axis.  Value is -180..180.  Controls the horizontal orientation")]
    [AxisStateProperty]
    public AxisState HorizontalAxis;

    [Header("Grappling Required Objects")]
    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform grappleTip, cam, car;

    [Header("Adjust Grappling Behaviour")]
    [Tooltip("How far to raycast to place the aim target")]
    public float maxGrapDistance = 200;
    private SpringJoint joint;
    public float spring = 8.5f;
    public float damper = 7f;
    public float massScale = 4.5f;

    /// <summary>
    /// Diese Methode überprüft nur die Werte auf gültigkeit
    /// </summary>
    private void OnValidate()
    {
        VerticalAxis.Validate();
        HorizontalAxis.Validate();
        AimDistance = Mathf.Max(1, AimDistance);
    }

    private void Reset()
    {
        AimDistance = 200;
        ReticleImage = null;
        CollideAgainst = 1;
        IgnoreTag = string.Empty;
        //hier noch hinkriegen das sich die kamera garnet bewegt
        // die minvalue und maxvalue einstellen
        VerticalAxis = new AxisState(-10, 35, false, false, 10f, 0.1f, 0.1f, "Mouse Y", true);
        VerticalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
        HorizontalAxis = new AxisState(-40, 40, true, false, 10f, 0.1f, 0.1f, "Mouse X", false);
        HorizontalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
    }

    private void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(PlaceReticle);
        CinemachineCore.CameraUpdatedEvent.AddListener(PlaceReticle);
    }

    private void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(PlaceReticle);
    }

    private void Update()
    {
        if (Brain == null)
            return;

        HorizontalAxis.Update(Time.deltaTime);
        VerticalAxis.Update(Time.deltaTime);

        PlaceTarget();
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }
    /// <summary>
    /// LateUpdate wird verwendet, da das Anzeigen der Rope nach der Grapple Logik geschehen soll
    /// </summary>
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



        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrapDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = car.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(car.position, grapplePoint);

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
    /// Quelle der Änderung: https://gamedev.stackexchange.com/questions/133038/unity-rotate-a-point-around-an-arbitrary-axis-and-anchor
    /// </summary>
    private void PlaceTarget()
    {
        Quaternion rotVertical = Quaternion.AngleAxis(VerticalAxis.Value, CarTransform.right);
        Quaternion rotHorizontal = Quaternion.AngleAxis(HorizontalAxis.Value, CarTransform.up);
        var camPos = Brain.CurrentCameraState.RawPosition;
        transform.position = GetProjectedAimTarget(camPos + rotVertical * rotHorizontal * CarTransform.forward, camPos); //macht kein unterschied ob ich car transform fwd oder vector 3 fwd
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="camPos"></param>
    /// <returns></returns>
    private Vector3 GetProjectedAimTarget(Vector3 pos, Vector3 camPos) //the relevant calculations are made here, how do i change it to what i need
    {
        var origin = pos;
        var fwd = (pos - camPos).normalized;
        //var fwd = CarTransform.forward;
        pos += AimDistance * fwd;
        if (CollideAgainst != 0 && RaycastIgnoreTag(
            new Ray(origin, fwd),
            out RaycastHit hitInfo, AimDistance, CollideAgainst))
        {
            pos = hitInfo.point;
        }
        return pos;
    }

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

    void PlaceReticle(CinemachineBrain brain)
    {
        if (brain == null || brain != Brain || ReticleImage == null || brain.OutputCamera == null)
            return;
        PlaceTarget(); // To eliminate judder
        CameraState state = brain.CurrentCameraState;
        var cam = brain.OutputCamera;
        var r = cam.WorldToScreenPoint(transform.position);
        var r2 = new Vector2(r.x - cam.pixelWidth * 0.5f, r.y - cam.pixelHeight * 0.5f);
        ReticleImage.anchoredPosition = r2;
    }
}
