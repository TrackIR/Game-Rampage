using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude, float smoothness = 0.1f)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        Vector3 currentOffset = Vector3.zero;
        Vector3 targetOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * magnitude;

        while (elapsed < duration)
        {
            // smoothly move towards the target offset
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, smoothness);

            transform.localPosition = originalPos + currentOffset;

            // pick a new target offset
            if ((currentOffset - targetOffset).sqrMagnitude < 0.01f)
            {
                targetOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * magnitude;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}