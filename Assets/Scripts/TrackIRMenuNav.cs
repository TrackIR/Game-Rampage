using JetBrains.Annotations;
using UnityEngine;

public class TrackIRMenuNav : MonoBehaviour
{
    public GameObject curserObject;
    public float sensitivity = 1;

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
        // cursor movement

        // get head rotation data
        Vector3 headRot = trackIR.LatestPoseOrientation.eulerAngles;
        
        // normalize rotation
        float normHorizontal = (WrapAngle(headRot.y) + 90) / 180;
        float normVertical = 1 - (WrapAngle(headRot.x) + 90) / 180;

        // convert normalized data into screen space
         Vector2 screenPos = new Vector2(
            normHorizontal * Screen.width,
            normVertical * Screen.height
        );

        // actually move the cursor object to screenPos
        curserObject.transform.position = screenPos;

        // click UI elements

        

    }
}
