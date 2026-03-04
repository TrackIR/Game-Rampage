using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class SpawnEnemies : MonoBehaviour
{
    // For now, placing the enemy spawner on the player so it moves relative to player position.
    public ManageUI ui; // Drag in the canvas from the player UI scene
    public GameObject enemyPrefab; // Change later once we have more enemies
    public NavMeshSurface navMeshSurface; // For updating the navmesh after spawning enemies
    public float spawnRate = 5f;
    public float randomOffset = 10; // How spread out enemy spawns are
    private float spawnTimer = 0f;
    public int spawnCount; // How many enemies to spawn each time
    public int radiusFromPlayer = 200; // How far from the player the enemies should spawn
    private float scalingFactor = 2;
    public LayerMask buildingMask; // Buildings that block spawning
    public float spawnerClearanceRadius = 2f; // Clearance check around spawner
    private Transform playerTransform;
    public GameSettings settings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshSurface.BuildNavMesh();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        scalingFactor = settings.difficulty == "Easy" ? 1 : settings.difficulty == "Hard" ? 3 : 2;
        spawnCount = Mathf.FloorToInt(scalingFactor * 4); // Adjust spawn count based on difficulty
    }

    void Update()
    {
        SpawnOnTimer();
    }

    void SpawnOnTimer()
    {
        float interval = Mathf.Clamp(spawnRate, 0.5f, 20f); // Can never be lower than 0.5 seconds

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= interval)
        {
            Vector3 center = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            float angleStep = 360f / Mathf.Max(1, spawnCount);
            float startAngle = Random.Range(0f, 360f);

            for (int i = 0; i < spawnCount; i++)
            {
                float angle = startAngle + (angleStep * i);
                float radians = angle * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * radiusFromPlayer;
                Vector3 spawnPos = center + offset;
                if (IsSpawnerClear(spawnPos))
                {
                    if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, randomOffset, NavMesh.AllAreas))
                    {
                        Instantiate(enemyPrefab, hit.position, Quaternion.identity);
                    }
                }
            }
            spawnTimer = 0f;
            scalingFactor = scalingFactor * 1.2f;
            spawnCount = Mathf.FloorToInt(scalingFactor * 4);
        }
    }

    bool IsSpawnerClear(Vector3 spawnPos)
    {
        return !Physics.CheckSphere(spawnPos, spawnerClearanceRadius, buildingMask, QueryTriggerInteraction.Ignore);
    }
}
