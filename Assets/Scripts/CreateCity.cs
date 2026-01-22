using UnityEngine;

public class CreateCity : MonoBehaviour
{

    public GameObject[] blockPrefabs; // List of prefabs, should use tile-001 as their base to prevent issues with pivots
    public GameObject roadPrefab; // In Assets/Prefabs/Worldtiles
    [Range(3f, 50f)] public int rows = 4;
    [Range(3f, 50f)] public int columns = 4;
    [Range(1, 3)] public int roadLayers = 1;

    private float offset;
    private int blockSize = 3; // Prefabs should use the tile-001 as a base, which is 3x3 tiles
    private float tileSpacing = 4f; // Roads are 4 units by 4 units


    void Start()
    {
        GenerateCity();
    }

    void GenerateCity()
    {
        offset = (blockSize + roadLayers * 2) * tileSpacing;

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
        int buildingStart = 0; // First tile of the building 3x3
        int buildingEnd = blockSize - 1; // Last tile of the building 3x3


        int roadStart = buildingStart - roadLayers; // First tile of the road
        int roadEnd = buildingEnd + roadLayers; // Last tile of the surrounding road

        for (int i = roadStart; i <= roadEnd; i++)
        {
            for (int j = roadStart; j <= roadEnd; j++)
            {
                bool isRoad = i < buildingStart || i > buildingEnd || j < buildingStart || j > buildingEnd; //Making sure that roads are not being placed on building tiles

                if (!isRoad)
                    continue;

                Vector3 pos = new Vector3(origin.x + i * tileSpacing, 0, origin.z + j * tileSpacing);
                Instantiate(roadPrefab, pos, Quaternion.identity, transform);
            }
        }
    }
}
