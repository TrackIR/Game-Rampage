using UnityEngine;
using Unity.Mathematics;

public class CreateCity : MonoBehaviour
{

    public GameObject[] blockPrefabs; // List of prefabs, should use tile-001 as their base to prevent issues with pivots
    public GameObject roadPrefab; // In Assets/Prefabs/Worldtiles
    public GameObject tilePrefab;
    [Range(3f, 50f)] public int rows = 4;
    [Range(3f, 50f)] public int columns = 4;
    [Range(1, 3)] public int roadLayers = 1;

    private float offset;
    [Range(3f, 100f)] public int blockSize = 9; // Number of tiles per block (should be a perfect square)
    private float roadWidth = 4f; // Width of a road tile
    private int tileSize = 4;


    void Start()
    {
        GenerateCity();
    }

    void GenerateCity()
    {
        offset = (blockSize + roadLayers * 2) * roadWidth;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 chunkOrigin = new Vector3(i * offset, 0, j * offset);
                PlaceBlock(chunkOrigin);
                PlaceRoad(chunkOrigin);
                PlaceBuilding(chunkOrigin);
            }
        }
    }

    void PlaceBlock(Vector3 origin)
    {
        for (int i = 0; i < blockSize; i++)
        {
            for (int j = 0; j < blockSize; j++)
            {
                Vector3 tilePos = new Vector3(origin.x + i * tileSize, 0, origin.z + j * tileSize);
                Instantiate(tilePrefab, tilePos, Quaternion.identity, transform);
            }
        }
    }

    // Places down a prefab from the list.
    void PlaceBuilding(Vector3 origin)
    {
        quaternion ranRotation = quaternion.EulerXYZ(0, math.radians(90 * UnityEngine.Random.Range(0, 4)), 0);
        for (int i = 0; i < math.sqrt(blockSize); i++)
        {
            for (int j = 0; j < math.sqrt(blockSize); j++)
            {
                // TODO: Currently places buildings too spread
                // Also: make new tile003 that isn't ass
                Vector3 newOrigin = new Vector3(origin.x + i * math.sqrt(blockSize) * tileSize, 0, origin.z + j * math.sqrt(blockSize) * tileSize);
                GameObject prefab = blockPrefabs[UnityEngine.Random.Range(0, blockPrefabs.Length)]; // Grab prefab from list
                GameObject block = Instantiate(prefab, newOrigin, ranRotation, transform); // Place the prefab down
                block.name = "Building_" + origin.x + "_" + newOrigin.z; // Name the prefab
            }
        }
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

                Vector3 pos = new Vector3(origin.x + i * roadWidth, 0, origin.z + j * roadWidth);
                Instantiate(roadPrefab, pos, Quaternion.identity, transform);
            }
        }
    }
}
