using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{

    // For now, placing the enemy spawner on the player so it moves relative to player position.

    public ManageUI ui; // Drag in the canvas from the player UI scene
    public GameObject enemyPrefab; // Change later once we have more enemies
    public float spawnRate = 10f;
    public float randomOffset = 10; // How spread out enemy spawns are
    public float maxEnemies = 3; // How many enemies can be spawned at one time

    private float spawnTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        SpawnOnTimer();
    }

    void SpawnOnTimer()
    {

        float interval = Mathf.Clamp(spawnRate - ui.difficulty, 0.5f, 20f); // Can never be lower than 0.5 seconds

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= interval)
        {
            int enemyNum = (int)Random.Range(1, maxEnemies);

            for (int i = 0; i < enemyNum; i++) // Spawn 1 to maxEnemies enemies
            {

                float randOffsetX = Random.Range(-randomOffset, randomOffset);
                float randOffsetZ = Random.Range(-randomOffset, randomOffset);

                // Calculate where the enemy should spawn, using random x and z values
                Vector3 spawnPos = new Vector3(transform.position.x + randOffsetX, transform.position.y, transform.position.z + randOffsetZ);

                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }
            spawnTimer = 0f;
        }
    }
}
