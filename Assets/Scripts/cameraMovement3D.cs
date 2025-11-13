using System;
using Mono.Cecil;
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

    [Header("3rd Person Settings")]
    public float distance = 5;
    public float verticalOffset = 8;
    public float horizontalOffsetWeight = 20;
    public float verticalOffsetWeight = 20;
    public float depthOffsetWeight = 25;
    public float smoothing = 5;

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
        Vector3 headRot = trackIR.LatestPoseOrientation.eulerAngles; // data from trackIR
        Vector3 targetPos;                                           // target position for camera

        // wrap all angles from trackIR to (-180, 180)
        headRot.x = WrapAngle(trackIRRoot.transform.rotation.x);
        headRot.y = WrapAngle(trackIRRoot.transform.rotation.y);
        headRot.z = WrapAngle(trackIRRoot.transform.rotation.z);

        // move target position based on head rotation
        targetPos.x = -headRot.y * horizontalOffsetWeight; // horizonal
        targetPos.y = headRot.x * verticalOffsetWeight;    // vertical
        targetPos.z = headRot.x * depthOffsetWeight;       // depth

        // orbit position offset
        float yawRads = math.radians(playerObject.transform.eulerAngles.y); // get y axis rotation in radians
        float xOffset = math.sin(yawRads) * distance;
        float zOffset = math.cos(yawRads) * distance;

        Vector3 orbitOffset = new Vector3(xOffset, verticalOffset, zOffset);

        // transforms the target position into proper rotated world space
        targetPos = playerObject.transform.rotation * targetPos;

        // take into account player's position and orbit offset
        targetPos = playerObject.transform.position + targetPos + orbitOffset;

        // clamp target position above ground
        targetPos.y = Mathf.Clamp(targetPos.y, 0.1f, 100f);

        // smoothly lerp position to target position
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);

        // set position to smoothed position
        transform.position = smoothPos;

        // copies player's y axis rotation to camera rotation
        transform.rotation = Quaternion.Euler(0f, playerObject.transform.rotation.eulerAngles.y, 0f);
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