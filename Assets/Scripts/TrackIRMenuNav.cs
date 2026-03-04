using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrackIRMenuNav : MonoBehaviour
{
    public GameObject curserObject;
    public KeyCode clickKey = KeyCode.Space;
    public float sensitivity = 1;

    TrackIRComponent trackIR;
    EventSystem eventSystem;
    PointerEventData pointerData;

    private float lastClickTime = 0f;
    private float clickCooldown = 0.1f;

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
        // Safety check to prevent null reference spam
        if (trackIR == null || eventSystem == null) return;

        Vector3 headRot = trackIR.LatestPoseOrientation.eulerAngles;
        float aspect = (float)Screen.width / Screen.height;

        float normHorizontal = ((WrapAngle(headRot.y) * sensitivity) + 90f) / 180f;
        float normVertical = 1f - (((WrapAngle(headRot.x) * sensitivity * aspect) + 90f) / 180f);

        Vector2 screenPos = new Vector2(
            normHorizontal * Screen.width,
            normVertical * Screen.height
        );

        if (curserObject != null)
        {
            curserObject.transform.position = screenPos;
        }
        pointerData.position = screenPos;

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, results);

        if (results.Count == 0)
        {
            // Clear the selection so that looking back at the button counts as a new hover
            if (eventSystem.currentSelectedGameObject != null)
            {
                eventSystem.SetSelectedGameObject(null);
            }
            return;
        }

        GameObject uiTarget = null;

        // Find the first actual interactable UI element (ignore text/backgrounds)
        foreach (RaycastResult result in results)
        {
            Selectable selectable = result.gameObject.GetComponentInParent<Selectable>();
            if (selectable != null && selectable.interactable)
            {
                uiTarget = selectable.gameObject;
                break; // Found a valid interactable button/selectable, stop looking
            }
        }

        // If hit nothing interactable (like the gap between keys)
        if (uiTarget == null)
        {
            if (eventSystem.currentSelectedGameObject != null)
            {
                eventSystem.SetSelectedGameObject(null);
            }
            return;
        }

        // Detect if hovered over a new interactable element
        if (eventSystem.currentSelectedGameObject != uiTarget)
        {
            eventSystem.SetSelectedGameObject(uiTarget);

            // Play Hover Sound
            if (AudioManager.Instance != null && AudioManager.Instance.menuHover != null)
            {
                AudioManager.Instance.playAudio(AudioManager.Instance.menuHover);
            }
        }

        // Click on element
        if (Input.GetKeyDown(clickKey))
        {
            if (Time.time - lastClickTime > clickCooldown)
            {
                lastClickTime = Time.time;

                // Play Click Sound
                if (AudioManager.Instance != null && AudioManager.Instance.menuClick != null)
                {
                    AudioManager.Instance.playAudio(AudioManager.Instance.menuClick);
                }
            }

            ExecuteEvents.Execute(uiTarget, pointerData, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(uiTarget, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(uiTarget, pointerData, ExecuteEvents.pointerClickHandler);
        }
    }
}