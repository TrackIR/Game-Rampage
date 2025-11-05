using System;
using System.IO.Compression;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class cameraMovement3D : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject cameraObject;

    public float distance = 5;
    public float defaultHight = 8;
    public float horizontalOffectWeight = 20;
    public float verticalOffectWeight = 20;
    public float depthOffectWeight = 15;


    // moves the camera target's empty
    void MoveTargetPosition()
    {
        // camera's rotation
        Vector3 camRot = new Vector3(cameraObject.transform.rotation.x,
                                     cameraObject.transform.rotation.y,
                                     cameraObject.transform.rotation.z);

        Vector3 basePos = new Vector3(0f, defaultHight, distance);  // default pos for camera
        Vector3 targetPos = basePos;                                // target pos for camera


        // x pos change with left/right head movement
        targetPos.x = basePos.x - (camRot.y * horizontalOffectWeight);

        // y pos change with up/down head movement
        targetPos.y = basePos.y + (camRot.x * verticalOffectWeight);

        // clamp targetPos.y so it never goes below
        // scretch goal to cast a ray down so it never goes below ground/debris
        Math.Clamp(targetPos.y, 0.1f, math.INFINITY);

        // z pos change tied to up/down head movement
        targetPos.z = basePos.z + (camRot.x * depthOffectWeight);

        transform.localPosition = targetPos;
        Debug.Log(camRot);
    }

    void Update()
    {
        MoveTargetPosition();
    }
}
