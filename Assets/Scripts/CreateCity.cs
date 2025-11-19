using UnityEngine;

public class CreateCity : MonoBehaviour
{

    public GameObject[] blockPrefabs; // List of prefabs, should use tile-001 as their base to prevent issues with pivots
    public GameObject roadPrefab; // In Assets/Prefabs/Worldtiles
    [Range(3f, 50f)] public int rows = 4;
    [Range(3f, 50f)] public int columns = 4;

    private float cityBlockSize; //Size of a full city block, including roads
    private float offset;
    private int blockSize = 3; // Prefabs should use the tile-001 as a base, which is 3x3 tiles
    private float tileSpacing = 4f; // Roads are 4 units by 4 units


    void Start()
    {
        cityBlockSize = blockSize * tileSpacing;
        GenerateCity();
    }

    void GenerateCity()
    {
        offset = (cityBlockSize + tileSpacing);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 chunkOrigin = new Vector3(i * offset, 0, j * offset);

                // Put down a building, then surround it with roads
                PlaceBlock(chunkOrigin);
                PlaceRoad(chunkOrigin);
            }
        }
    }

    // Places down a prefab from the list.
    void PlaceBlock(Vector3 origin)
    {
        GameObject prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)]; // Grab prefab from list

        GameObject block = Instantiate(prefab, origin, Quaternion.identity, transform); // Place the prefab down
        block.name = "Block_" + origin.x + "_" + origin.z; // Name the prefab
    }


    // Places down a border of roads around a block
    void PlaceRoad(Vector3 origin)
    {
        for (int i = -1; i <= blockSize; i++)
        {
            for (int j = -1; j <= blockSize; j++)
            {
                bool border = i == -1 || i == blockSize || j == -1 || j == blockSize;

                if (!border)
                {
                    continue;  // If not a border, move on
                }

                Vector3 pos = new Vector3(origin.x + i * tileSpacing, 0, origin.z + j * tileSpacing);

                Instantiate(roadPrefab, pos, Quaternion.identity, transform);
            }
        }
    }
}
