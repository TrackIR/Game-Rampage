using System;
using Unity.Mathematics;
using UnityEngine;

public class cameraMovement3D : MonoBehaviour
{
    public GameSettings gameSettings;

    [Header("Camera Settings")]
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

    [Header("Mouse Look (Absolute)")]
    public float yawRange = 180f;   // full left/right range
    public float maxPitch = 120f;    // up/down clamp

    // unified input
    private float inputYaw;
    private float inputPitch;

    void Start()
    {
        trackIR = trackIRRoot.GetComponent<TrackIRComponent>();
    }

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
        Quaternion headRotation;

        // input layer -- still testing, likely needs extra rotation
        if (gameSettings.useTrackIR)
        {
            // USING TRACK-IR
            Quaternion childRotation = trackIR.LatestPoseOrientation;

            Vector3 headEuler = childRotation.eulerAngles;

            inputYaw = WrapAngle(headEuler.y);
            inputPitch = WrapAngle(headEuler.x);

            headRotation = Quaternion.Euler(inputPitch, inputYaw, 0f);
        }
        else
        {
            // USING MOUSE
            float nx = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
            float ny = -(Input.mousePosition.y / Screen.height - 0.5f) * 2f;

            inputYaw = nx * yawRange;
            inputPitch = Mathf.Clamp(ny * maxPitch, -maxPitch, maxPitch);

            Quaternion yawRot = Quaternion.AngleAxis(inputYaw, Vector3.up);
            Quaternion pitchRot = Quaternion.AngleAxis(inputPitch, Vector3.right);

            headRotation = yawRot * pitchRot;
        }

        // 3rd person
        float orbitYaw = inputYaw * yawOrbitWeight;
        float orbitPitch = inputPitch * pitchOrbitWeight;

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

        Quaternion thirdPersonRot;
        if (gameSettings.useTrackIR)
        {
            thirdPersonRot = worldLookRotation * Quaternion.Inverse(headRotation);
        }
        else
        {
            thirdPersonRot = worldLookRotation;
        }

        // 1st person
        Vector3 firstPersonPos =
            playerObject.transform.position +
            playerObject.transform.rotation * firstPersonOffset;

        Quaternion firstPersonRot =
            playerObject.transform.rotation * headRotation;

        // blending
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
        targetBlend = is3rdPerson ? 1f : 0f;

        cameraBlend = Mathf.MoveTowards(
            cameraBlend,
            targetBlend,
            transitionSpeed * Time.deltaTime
        );

        MoveBlendedCamera();
    }
}