using JetBrains.Annotations;
using UnityEngine;

public class TrackIRMenuNav : MonoBehaviour
{
    public GameObject curserObject;
    public float sensitivity;

    // private variables
    TrackIRComponent trackIR;

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
    }

    void Update()
    {
        Vector3 headRot = trackIR.LatestPoseOrientation.eulerAngles;

        float normHorizontal = (WrapAngle(headRot.y) + 90) / 180;
        float normVertical = 1 - (WrapAngle(headRot.x) + 90) / 180;

        Vector2 cursorPos = new Vector2(normHorizontal * Screen.width, normVertical * Screen.height);

        Debug.Log(cursorPos);
    }
}
