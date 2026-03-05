using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    public GameObject[] prefabs; // List of buildings to place
    private float placeChance = 0.9f; // Should be fairly high

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
        int ranDirection = Random.Range(0, 4);
        Quaternion rotation = Quaternion.Euler(0, ranDirection * 90, 0);

        Instantiate(prefab, position, rotation, transform);
    }
}
