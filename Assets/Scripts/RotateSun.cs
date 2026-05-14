using UnityEngine;

public class RotateSun : MonoBehaviour
{

    [Range(0.01f, 1f)] public float rate = 0.001f;

    // Update is called once per frame
    void Update()
    {
        // Rotate the sun around the game area slowly
        transform.Rotate(rate * Time.deltaTime, 0f, 0f);
    }
}
