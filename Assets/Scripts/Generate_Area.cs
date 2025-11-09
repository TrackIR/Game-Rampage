using UnityEngine;

public class Generate_Area : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject roadPrefab;
    public int rows = 5;
    public int columns = 5;
    public float spacing = 8f; // Puts a bit of space between the tiles
    public int blockSize = 3; // Controls size of city blocks

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                if (x % (blockSize + 1) == 0 || z % (blockSize + 1) == 0)
                {
                    Vector3 position = new Vector3(x * spacing, 0, z * spacing); // Add spacing to tiles
                    GameObject obj = Instantiate(roadPrefab, position, Quaternion.identity, transform); // Put down a road
                }
                else 
                {
                    Vector3 position = new Vector3(x * spacing, 0, z * spacing); // Add spacing to tiles
                    GameObject obj = Instantiate(tilePrefab, position, Quaternion.identity, transform); // Put down a building tile
                }
            }
        }

        // Put an object on the grid space
       PlaceObject placeObject = GetComponent<PlaceObject>();
        if (placeObject != null)
        {
            placeObject.place_object();
        }
        else
        {
            print("Missing object"); // Should only print if list has missing parts or is empty
        }

    }
}
