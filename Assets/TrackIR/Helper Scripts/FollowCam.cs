// *******************************************************************************
// * Copyright 2023, NaturalPoint Inc.
// *******************************************************************************
// * Description:
// *
// *   Camera component to swap between third and first person points of view of a target object.
// *   Since TrackIR updates camera position relatively, we need parent object to
// *   move with a character/player.
// * 
// *******************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Media;
using System.Security.Cryptography;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class FollowCam : MonoBehaviour
{

    // third person variables
    public Transform mFirstPersonFollowTarget;
    public Transform mThirdPersonFollowTarget;
    public float distance = 2f;
    public float height = 0.5f;
    public float rotationSpeed = 100f;
    public float cameraSmoothSpeed = 10f;

    // first person variables
    public Vector3 firstPersonCameraOffest = new Vector3(0, 0, 0);

    private Quaternion previousTargetRotation;

    private bool isThirdPerson = true;

    void Start()
    {
        previousTargetRotation = mFirstPersonFollowTarget.rotation;
    }

    void Update()
    {
        if(mFirstPersonFollowTarget == null)
        {
            return;
        }

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            isThirdPerson = !isThirdPerson;
            previousTargetRotation = mFirstPersonFollowTarget.rotation;
        }
#else
        if (Input.GetKeyDown(KeyCode.C))
        {
            isThirdPerson = !isThirdPerson;
            previousTargetRotation = mFirstPersonFollowTarget.rotation;
        }
#endif

        // third person camera
        if (isThirdPerson)
        {
            // Calculate the desired position of the camera
            Vector3 targetPosition = mThirdPersonFollowTarget.position - mThirdPersonFollowTarget.forward * distance + mThirdPersonFollowTarget.up * height;

            // Move the camera towards the desired position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSmoothSpeed);

            transform.LookAt(mThirdPersonFollowTarget);

            // Rotate the camera around the mFirstPersonFollowTarget's y-axis based on time
            float rotationAmount = Time.deltaTime;
            Quaternion currentRotation = transform.rotation;
            transform.RotateAround(mThirdPersonFollowTarget.position, Vector3.up, rotationAmount);
            Quaternion newRotation = transform.rotation;
            newRotation.eulerAngles = new Vector3(0f, newRotation.eulerAngles.y, 0f);
            transform.rotation = newRotation;

        }else if(!isThirdPerson)
        {
            Vector3 targetPosition = mFirstPersonFollowTarget.position + firstPersonCameraOffest;
            transform.position = targetPosition;

            // Rotate the camera based on the target's rotation
            float targetRotationDelta = mFirstPersonFollowTarget.rotation.eulerAngles.y - previousTargetRotation.eulerAngles.y;
            Quaternion targetRotationDeltaQuaternion = Quaternion.Euler(0, targetRotationDelta, 0);
            transform.rotation = targetRotationDeltaQuaternion * transform.rotation;

            previousTargetRotation = mFirstPersonFollowTarget.rotation;



        }

    }

}
