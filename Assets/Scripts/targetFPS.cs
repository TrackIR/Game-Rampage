using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int targetFPS = 60;

    void Awake()
    {
        Application.targetFrameRate = targetFPS;
        // disable VSync to ensure targetFrameRate takes full effect
        QualitySettings.vSyncCount = 0;
    }
}
