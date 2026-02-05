using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrackIRMenuNav : MonoBehaviour
{
    public GameObject curserObject;
    public KeyCode clickKey = KeyCode.Space; // replace with actually input system later
    public float sensitivity = 1;

    // private variables
    TrackIRComponent trackIR;
    EventSystem eventSystem;
    PointerEventData pointerData;

    // turns '0 to 360' degrees into '-180 to 180'
    float WrapAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }
        return angle;
    }

    void Start()
    {
        trackIR = GetComponent<TrackIRComponent>();
        eventSystem = EventSystem.current;
        pointerData = new PointerEventData(eventSystem);
    }

    void Update()
    {
        // cursor movement

        // get head rotation data
        Vector3 headRot = trackIR.LatestPoseOrientation.eulerAngles;

        // aspect ratio compensation
        float aspect = (float)Screen.width / Screen.height;

        // normalize rotation
        float normHorizontal = ((WrapAngle(headRot.y) * sensitivity) + 90f) / 180f;
        float normVertical = 1f - (((WrapAngle(headRot.x) * sensitivity * aspect) + 90f) / 180f);

        // convert normalized data into screen space
        Vector2 screenPos = new Vector2(
            normHorizontal * Screen.width,
            normVertical * Screen.height
        );

        // actually move the cursor object to screenPos
        curserObject.transform.position = screenPos;

        // click UI elements
        pointerData.position = screenPos;

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, results);

        // if nothing is hit, stop
        if (results.Count == 0)
        {
            return;
        }

        // element under cursor
        GameObject uiTarget = results[0].gameObject;

        //Debug.Log(uiTarget.name);

        // hover element
        eventSystem.SetSelectedGameObject(uiTarget);

        // click on element
        if (Input.GetKeyDown(clickKey))
        {
            Debug.Log(uiTarget.name);

            ExecuteEvents.Execute(uiTarget, pointerData, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(uiTarget, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(uiTarget, pointerData, ExecuteEvents.pointerClickHandler);
        }

    }
}
