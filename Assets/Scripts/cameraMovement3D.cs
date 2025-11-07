using System;
using System.IO.Compression;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class cameraMovement3D : MonoBehaviour
{
    [Header("Camera Settings")]
    // trackIR componments
    TrackIRComponent trackIR;
    public GameObject trackIRRoot;

    public GameObject playerObject;
    public GameObject cameraObject;
    public bool is3rdPerson = true;

    [Header("3rd Person Settings")]
    public float distance = 5;
    public float defaultHight = 8;
    public float horizontalOffectWeight = 20;
    public float verticalOffectWeight = 20;
    public float depthOffectWeight = 25;

    [Header("1st Person Settings")]
    public float xOffset;
    public float yOffset;
    public float zOffset;

    void Start()
    {
        trackIR = trackIRRoot.GetComponent<TrackIRComponent>();
    }

    // moves the camera target's empty
    void Move3rdCamTargetPosition()
    {
        // Extract raw Euler angles (0–360)
        Vector3 headRot = trackIR.LatestPoseOrientation.eulerAngles;

        // Convert each axis to -180–180
        if (headRot.x > 180f) headRot.x -= 360f;
        if (headRot.y > 180f) headRot.y -= 360f;
        if (headRot.z > 180f) headRot.z -= 360f;

        Vector3 basePos = new Vector3(0f, defaultHight, distance);
        Vector3 targetPos = basePos;

        // Horizontal movement: use signed yaw
        targetPos.x = basePos.x - (headRot.y * horizontalOffectWeight);

        // Vertical movement: use signed pitch
        targetPos.y = basePos.y + (headRot.x * verticalOffectWeight);

        // Clamp so camera target never dips below ground
        targetPos.y = Mathf.Clamp(targetPos.y, 1f, float.PositiveInfinity);

        // Depth movement: also based on pitch
        targetPos.z = basePos.z + (headRot.x * depthOffectWeight);

        transform.localPosition = targetPos;
    }


    void Move1stCamTargetPosition()
    {
        Vector3 targetPos = new Vector3(xOffset, yOffset, zOffset);
        transform.localPosition = playerObject.transform.position + targetPos;
    }

    void Update()
    {
        if (is3rdPerson) {
            Move3rdCamTargetPosition();
        } else
        {
            Move1stCamTargetPosition();
        }
    }
}
