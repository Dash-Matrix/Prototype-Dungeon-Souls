using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [Header("Spawning Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;
    public int maxEnemiesAlive = 5;

    public float increaseInterval = 60f;  // Every 1 minute
    public int increaseAmount = 1;        // Increase by how much

    private int currentEnemies = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
        StartCoroutine(IncreaseMaxEnemiesOverTime());
    }

    void SpawnEnemy()
    {
        if (currentEnemies >= maxEnemiesAlive) return;

        Transform spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPos.position, Quaternion.identity);

        currentEnemies++;
    }

    public void HandleEnemyDeath()
    {
        currentEnemies--;
    }

    private System.Collections.IEnumerator IncreaseMaxEnemiesOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(increaseInterval);
            maxEnemiesAlive += increaseAmount;
            Debug.Log("Max Enemies Increased To: " + maxEnemiesAlive);
        }
    }
}
