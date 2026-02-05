using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    public GameObject[] prefabs; // List of buildings to place
    private float placeChance = 0.9f; // Should be fairly high
    private float randomOffset = 0.1f; // Shifts objects around a bit

    public void Start()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            return;
        }

        if (Random.value >= placeChance)
        {
            return;
        }
        // Get a random prefab from the list
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        // Get the postion and apply random offsets
        Vector3 position = transform.position;
        position.x += Random.Range(-randomOffset, randomOffset);
        position.z += Random.Range(-randomOffset, randomOffset);

        Instantiate(prefab, position, Quaternion.identity, transform);
    }
}
