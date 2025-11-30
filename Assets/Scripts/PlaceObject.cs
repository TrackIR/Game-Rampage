using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    public GameObject[] prefabs; // List of buildings to place
    [Range(0f, 1f)] public float placeChance = 0.1f; // Should be fairly low
    [Range(0f, 5f)] public float randomOffset = 0.1f; // Shifts objects around a bit
    [Range(-5f, 5f)] public float yOffset = 1f; // Y offset if buildings are in tiles

    public void place_object()
    {
        foreach (Transform child in transform)
        {
            if (!child.CompareTag("BuildingTile")) // Check to make sure that tile has the right tag (not the road)
                continue;

            if (Random.value <= placeChance)
            {
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)]; // Get prefab from the list

                // Add a random x and z offset for some variation in building position
                Vector3 randomPos = new Vector3(Random.Range(-randomOffset, randomOffset), 0f, Random.Range(-randomOffset, randomOffset));


                Vector3 objectPos = child.position + Vector3.up * yOffset + randomPos; // Position of building, based on tile position


                GameObject placedObject = Instantiate(prefab, objectPos, Quaternion.identity); // Place the building
                placedObject.transform.SetParent(child, true); // Set the new object as a child of the level tile
            }
        }
    }
}
