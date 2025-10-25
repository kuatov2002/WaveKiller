using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RandomEnemyIndicatorSpawner : MonoBehaviour
{
    [Tooltip("Префаб объекта, который будет создаваться над врагом")]
    public GameObject indicatorPrefab;

    [Tooltip("Интервал между попытками (в секундах)")]
    public float spawnInterval = 3f;

    [Tooltip("Высота над врагом, на которой будет появляться объект")]
    public float heightOffset = 1.5f;

    private List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnIndicatorRoutine());
    }

    System.Collections.IEnumerator SpawnIndicatorRoutine()
    {
        while (true)
        {
            // Обновляем список врагов (на случай появления/исчезновения)
            enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();

            if (enemies.Count > 0)
            {
                // Выбираем случайного врага
                GameObject randomEnemy = enemies[Random.Range(0, enemies.Count)];

                // Позиция над врагом
                Vector3 spawnPosition = randomEnemy.transform.position + Vector3.up * heightOffset;

                // Создаём объект
                Instantiate(indicatorPrefab, spawnPosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}