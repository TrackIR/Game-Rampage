using System;
using System.IO.Compression;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class cameraMovement3D : MonoBehaviour
{
    public bool is3rdPerson = true;

    public GameObject playerObject;
    public GameObject cameraObject;
    public GameObject trackIRRoot;
    
    TrackIRComponent trackIR;

    public float distance = 5;
    public float defaultHight = 8;
    public float horizontalOffectWeight = 20;
    public float verticalOffectWeight = 20;
    public float depthOffectWeight = 25;

    Quaternion headRot;


    // moves the camera target's empty
    void Move3DTargetPosition()
    {

        headRot = trackIR.LatestPoseOrientation;

        // camera's rotation
        Vector3 camRot = new Vector3(headRot.x, headRot.y, headRot.z);

        Vector3 basePos = new Vector3(0f, defaultHight, distance);  // default pos for camera
        Vector3 targetPos = basePos;                                // target pos for camera


        // x pos change with left/right head movement
        targetPos.x = basePos.x - (camRot.y * horizontalOffectWeight);

        // y pos change with up/down head movement
        targetPos.y = basePos.y + (camRot.x * verticalOffectWeight);

        Math.Clamp(targetPos.y, 1f, math.INFINITY); // clamp targetPos.y so it never goes below ground level; scretch goal to cast a ray down so it never goes below ground/debris

        // z pos change tied to up/down head movement
        targetPos.z = basePos.z + (camRot.x * depthOffectWeight);

        // set empty's position to target position
        transform.localPosition = targetPos;
    }

    void Start()
    {
        trackIR = trackIRRoot.GetComponent<TrackIRComponent>();
    }

    void Update()
    {
        if (is3rdPerson) {
            Move3DTargetPosition();
        }
    }
}
