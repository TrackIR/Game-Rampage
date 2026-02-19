using System;
using Unity.Mathematics;
using UnityEngine;

public class cameraMovement3D : MonoBehaviour
{
    [Header("Camera Settings")]
    // trackIR componments
    TrackIRComponent trackIR;
    public GameObject trackIRRoot;

    public GameObject playerObject;
    public bool is3rdPerson = true;

    [Header("3rd Person Orbit Settings")]
    public float distance = 5f;
    public Vector3 centerOffset = new Vector3(0f, 2f, 0f);
    public float yawOrbitWeight = 1f;
    public float pitchOrbitWeight = 1f;
    public float lookRotationWeight = 1f;

    public float positionSmoothing = 5f;
    public float rotationSmoothing = 5f;


    [Header("1st Person Settings")]
    public float xOffset;
    public float yOffset;
    public float zOffset;

    void Start()
    {
        trackIR = trackIRRoot.GetComponent<TrackIRComponent>();
    }

    // turns '0 to 360' degrees into '-180 to 180'
    float WrapAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }
        return angle;
    }

    // moves the camera target's empty
    void Move3rdCamTargetTransform()
    {
        // get TrackIR rotation
        Quaternion childRotation = trackIR.LatestPoseOrientation;

        Vector3 headEuler = childRotation.eulerAngles;
        float headYaw = WrapAngle(headEuler.y);
        float headPitch = WrapAngle(headEuler.x);

        // apply orbit scaling
        float orbitYaw = headYaw * yawOrbitWeight;
        float orbitPitch = headPitch * pitchOrbitWeight;

        float yawRad = orbitYaw * Mathf.Deg2Rad;
        float pitchRad = orbitPitch * Mathf.Deg2Rad;

        // compute orbit position relative to player rotation
        Vector3 localOrbit;
        localOrbit.x = distance * Mathf.Cos(pitchRad) * Mathf.Sin(yawRad);
        localOrbit.y = distance * Mathf.Sin(pitchRad);
        localOrbit.z = distance * Mathf.Cos(pitchRad) * Mathf.Cos(yawRad);

        // rotate orbit into player's facing direction
        Vector3 centerPoint =
            playerObject.transform.position +
            playerObject.transform.rotation * centerOffset;

        Vector3 worldOrbitOffset =
            playerObject.transform.rotation * localOrbit;

        Vector3 desiredWorldPosition =
            centerPoint + worldOrbitOffset;

        // compute world look rotation toward center
        Vector3 lookDir = (centerPoint - desiredWorldPosition).normalized;
        Quaternion worldLookRotation =
            Quaternion.LookRotation(lookDir, Vector3.up);

        // apply look weight (over/under rotate)
        worldLookRotation = Quaternion.Slerp(
            Quaternion.identity,
            worldLookRotation,
            lookRotationWeight
        );

        // cancel TrackIR camera rotation properly (world-space math)
        Quaternion desiredParentWorldRotation =
            worldLookRotation * Quaternion.Inverse(childRotation);

        // smooth position
        transform.position = Vector3.Lerp(
            transform.position,
            desiredWorldPosition,
            positionSmoothing * Time.deltaTime
        );

        // smooth rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredParentWorldRotation,
            rotationSmoothing * Time.deltaTime
        );
    }


    void Move1stCamTargetTransform()
    {
        Vector3 targetPos = new Vector3(xOffset, yOffset, zOffset);
        transform.position = playerObject.transform.position + targetPos;
    }

    void Update()
    {
        if (is3rdPerson)
        {
            Move3rdCamTargetTransform();
        }
        else
        {
            Move1stCamTargetTransform();
        }
    }
}