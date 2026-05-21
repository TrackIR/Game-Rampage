using UnityEngine;

public class LaserColorShift : MonoBehaviour
{

    // Public Laser Variable
    [Header("Color")]
    public Color startColor = Color.white;
    public Color endColor = Color.white;

    [Header("Color Shift Settings")]
    [Range(0.0f, 5.0f)]
    public float duration = 1.0f;

    private Renderer[] laserRends;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        laserRends = GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

        // If the player is using their ultimate and the laser renderers were found, cycle colors
        if (laserRends != null)
        {
            CycleLaser();
        }
    }

    // Cycle the laser material between the start and end colors
    private void CycleLaser()
    {
        // taken from Material.color in unity documentation 
        // https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Material-color.html

        float lerp = Mathf.PingPong(Time.time, duration);

        foreach (Renderer rend in laserRends)
        {
            if (rend != null)
            {
                rend.material.color = Color.Lerp(startColor, endColor, lerp);
            }
        }

    }
}

