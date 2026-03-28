using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private GameObject[] enemyPrefabs;      // 스폰할 에너미 프리팹 목록
    [SerializeField] private int maxEnemyCount = 10;         // 최대 동시 스폰 수
    [SerializeField] private float spawnInterval = 3f;       // 스폰 간격 (초)
    [SerializeField] private float spawnRadius = 10f;        // 플레이어 기준 스폰 반경
    [SerializeField] private float minSpawnDistance = 4f;    // 플레이어와 최소 거리

    private Transform player;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            CleanUpDeadEnemies();

            if (activeEnemies.Count < maxEnemyCount)
                SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPos = GetRandomSpawnPosition();
        if (spawnPos == Vector2.zero) return;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(enemy);
    }

    private Vector2 GetRandomSpawnPosition()
    {
        // 최대 10번 시도해서 유효한 위치 탐색
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 candidate = (Vector2)player.position + randomOffset;

            // 플레이어와 너무 가까우면 스킵
            if (Vector2.Distance(candidate, player.position) < minSpawnDistance)
                continue;

            return candidate;
        }

        return Vector2.zero; // 유효한 위치 못 찾으면 스폰 안 함
    }

    // 죽은 에너미를 리스트에서 정리
    private void CleanUpDeadEnemies()
    {
        activeEnemies.RemoveAll(e => e == null);
    }
}