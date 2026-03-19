using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    public GameObject[] prefabs; // List of buildings to place
    public GameObject parkPrefab;
    private float placeChance = 1.0f; // Should be fairly high, 

    public void Start()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            return;
        }

        Vector3 position = transform.position;
        int ranDirection = Random.Range(0, 4);
        // Get the postion and apply random offsets
        Quaternion rotation = Quaternion.Euler(0, ranDirection * 90, 0);
        if (Random.value >= placeChance)
        {
            // Replace empty lot with park
            //Instantiate(parkPrefab, position, rotation, transform);
            return;
        }
        // Get a random prefab from the list
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

        Instantiate(prefab, position, rotation, transform);
    }
}
