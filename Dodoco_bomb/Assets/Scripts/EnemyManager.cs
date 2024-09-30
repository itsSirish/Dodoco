using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int maxEnemiesOnBoard = 4;
    public int totalEnemiesToSpawn = 30;
    private int enemiesSpawned = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        SpawnEnemies();
    }

    private void Update()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);

        if (activeEnemies.Count < maxEnemiesOnBoard && enemiesSpawned < totalEnemiesToSpawn)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        int enemiesToSpawn = maxEnemiesOnBoard - activeEnemies.Count;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (enemiesSpawned >= totalEnemiesToSpawn)
                return;

            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            activeEnemies.Add(newEnemy);
            enemiesSpawned++;
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }
}
