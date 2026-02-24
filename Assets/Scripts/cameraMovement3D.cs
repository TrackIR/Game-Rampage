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
    public Vector3 firstPersonOffset;

    [Header("Transition")]
    [Range(0f, 1f)]
    public float cameraBlend = 1f;   // 1 = 3rd person, 0 = 1st person
    public float transitionSpeed = 3f;
    private float targetBlend = 1f;

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

    void MoveBlendedCamera()
    {
        // 3rd person

        Quaternion childRotation = trackIR.LatestPoseOrientation;

        Vector3 headEuler = childRotation.eulerAngles;
        float headYaw = WrapAngle(headEuler.y);
        float headPitch = WrapAngle(headEuler.x);

        float orbitYaw = headYaw * yawOrbitWeight;
        float orbitPitch = headPitch * pitchOrbitWeight;

        float yawRad = orbitYaw * Mathf.Deg2Rad;
        float pitchRad = orbitPitch * Mathf.Deg2Rad;

        Vector3 localOrbit;
        localOrbit.x = distance * Mathf.Cos(pitchRad) * Mathf.Sin(yawRad);
        localOrbit.y = distance * Mathf.Sin(pitchRad);
        localOrbit.z = distance * Mathf.Cos(pitchRad) * Mathf.Cos(yawRad);

        Vector3 centerPoint =
            playerObject.transform.position +
            playerObject.transform.rotation * centerOffset;

        Vector3 worldOrbitOffset =
            playerObject.transform.rotation * localOrbit;

        Vector3 thirdPersonPos = centerPoint + worldOrbitOffset;

        Vector3 lookDir = (centerPoint - thirdPersonPos).normalized;
        Quaternion worldLookRotation =
            Quaternion.LookRotation(lookDir, Vector3.up);

        worldLookRotation = Quaternion.Slerp(
            Quaternion.identity,
            worldLookRotation,
            lookRotationWeight
        );

        Quaternion thirdPersonRot =
            worldLookRotation * Quaternion.Inverse(childRotation);


        // 1st person

        Vector3 firstPersonPos =
            playerObject.transform.position +
            playerObject.transform.rotation * firstPersonOffset;

        //Quaternion firstPersonRot =
            //playerObject.transform.rotation *
            //Quaternion.Inverse(trackIR.LatestPoseOrientation);

        Quaternion firstPersonRot =
            playerObject.transform.rotation;


        // blend

        Vector3 blendedPos = Vector3.Lerp(
            firstPersonPos,
            thirdPersonPos,
            cameraBlend
        );

        Quaternion blendedRot = Quaternion.Slerp(
            firstPersonRot,
            thirdPersonRot,
            cameraBlend
        );

        transform.position = blendedPos;
        transform.rotation = blendedRot;
    }

    void Update()
    {
        // determine target blend
        targetBlend = is3rdPerson ? 1f : 0f;

        // smooth blend value
        cameraBlend = Mathf.MoveTowards(
            cameraBlend,
            targetBlend,
            transitionSpeed * Time.deltaTime
        );

        MoveBlendedCamera();
    }
}