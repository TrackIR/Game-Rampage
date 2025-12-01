using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public ManageUI ui; // Drag in the canvas from the player UI scene
    public GameObject enemyPrefab; // Change later once we have more enemies
    public float spawnRate = 10f;

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

        float interval = Mathf.Clamp(spawnRate - ui.difficulty, 0.5f, 20f);

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= interval)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            spawnTimer = 0f;
        }
    }
}
