using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class SpawnEnemies : MonoBehaviour
{
    [Header("Game Objects")]
    public ManageUI ui; // Drag in the canvas from the player UI scene
    public GameObject enemyPrefab; // Change later once we have more enemies
    public NavMeshSurface navMeshSurface; // For updating the navmesh after spawning enemies
    public GameSettings settings;
    public LayerMask buildingMask; // Buildings that block spawning
    [Header("Spawn Settings")]
    public float spawnRate = 10f; //How often enemies spawn in seconds
    private float spawnTimer = 0f;
    public int spawnCount; // How many enemies to spawn each time
    public float maxEnemies = 3;
    private float scalingFactor = 2;
    public int radiusFromPlayer = 200; // How far from the player the enemies should spawn
    public float randomOffset = 10; // How spread out enemy spawns are
    public float spawnerClearanceRadius = 2f; // Clearance check around spawner
    private Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshSurface.BuildNavMesh();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        scalingFactor = settings.difficulty == "Easy" ? 1 : settings.difficulty == "Hard" ? 3 : 2;
        spawnCount = Mathf.FloorToInt(scalingFactor * 2); // Adjust spawn count based on difficulty
        spawnTimer = spawnRate; // So enemies start spawning immediately
    }

    void Update()
    {
        SpawnOnTimer();
    }

    void SpawnOnTimer()
    {
        // Default settings for standard game modes
        float activeDifficultyModifier = ui.difficulty;
        float activeMaxEnemies = maxEnemies;

        // TRADE SHOW SCALING
        if (ui.isTradeShow)
        {
            // Threat level 1-5 aggressively reduces the wait time between spawns
            activeDifficultyModifier = ui.wantedLevel * 1.5f;

            // Threat level dynamically increases the swarm size! (Level 5 adds 5 extra max enemies)
            activeMaxEnemies = maxEnemies + ui.wantedLevel;
        }

        // Calculate the interval, clamping it so it never fires faster than twice a second
        float interval = Mathf.Clamp(spawnRate - activeDifficultyModifier, 0.5f, 20f);

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
            scalingFactor = scalingFactor * 1.1f;
            spawnCount = Mathf.FloorToInt(scalingFactor * 2);
        }
    }

    bool IsSpawnerClear(Vector3 spawnPos)
    {
        return !Physics.CheckSphere(spawnPos, spawnerClearanceRadius, buildingMask, QueryTriggerInteraction.Ignore);
    }
}