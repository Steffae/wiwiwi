using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 5f;        // Интервал между спавнами
    public int maxEnemies = 10;             // Максимальное количество врагов одновременно
    public float spawnRadius = 10f;         // Радиус спавна вокруг точки

    [Header("Spawn Area")]
    public bool useRandomPoint = true;      // Использовать случайную точку
    public Vector3 customSpawnArea;         // Размер области спавна

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float lastSpawnTime;

    void Start()
    {
        lastSpawnTime = Time.time;
    }

    void Update()
    {
        // Очищаем список от уничтоженных врагов
        activeEnemies.RemoveAll(enemy => enemy == null);

        // Спавним новых врагов
        if (Time.time > lastSpawnTime + spawnInterval && activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("Нет префабов врагов для спавна!");
            return;
        }

        // Выбираем случайного врага из массива
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[randomIndex];

        // Вычисляем позицию спавна
        Vector3 spawnPosition = GetSpawnPosition();

        // Создаём врага
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);

        Debug.Log($"Спавнен враг {enemyPrefab.name} на позиции {spawnPosition}");
    }

    Vector3 GetSpawnPosition()
    {
        if (useRandomPoint)
        {
            // Случайная точка в радиусе от спавнера
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            return transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
        else
        {
            // Случайная точка в прямоугольной области
            float randomX = Random.Range(-customSpawnArea.x / 2, customSpawnArea.x / 2);
            float randomZ = Random.Range(-customSpawnArea.z / 2, customSpawnArea.z / 2);
            return transform.position + new Vector3(randomX, 0, randomZ);
        }
    }

    // Визуализация зоны спавна в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        if (useRandomPoint)
        {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, customSpawnArea);
        }

        // Точка спавнера
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}