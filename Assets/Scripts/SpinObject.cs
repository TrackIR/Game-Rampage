using UnityEngine;

public class SpinObject : MonoBehaviour
{
    public float spinRate;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, spinRate * Time.deltaTime);
    }
}
