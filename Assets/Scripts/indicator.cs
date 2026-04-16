using UnityEngine;

public class indicator : MonoBehaviour
{
    Canvas canvas;
    public GameSettings gameSettings;
    public Camera trackIRCam;
    public Camera normal3rdCam;
    private Camera cam;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        if (gameSettings.useTrackIR)
        {
            canvas.worldCamera = trackIRCam;
        }
        else
        {
            canvas.worldCamera = normal3rdCam;
        }
        cam = canvas.worldCamera;
    }

    void Update()
    {
        transform.forward = cam.transform.forward;
        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time * 2f) * 0.01f, transform.position.z);
    }
}
