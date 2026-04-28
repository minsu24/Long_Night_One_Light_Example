using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("일반 스폰 설정")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int maxEnemyCount = 10;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minSpawnDistance = 4f;

    [Header("환각 몬스터 설정")]
    [SerializeField] private GameObject[] hallucinationPrefabs;  // 환각 전용 프리팹 (없으면 일반 프리팹 사용)

    // 정신력 단계별 환각 스폰 확률: [Normal, Depression, Anxiety, Collapse]
    // 붕괴 구간으로 갈수록 환각 빈도가 점진적으로 증가
    // Normal=0%, Depression=0%, Anxiety=50%, Collapse=80% — 불안 단계부터 환각 스폰
    [SerializeField] private float[] hallucinationChancePerState = { 0f, 0f, 0.5f, 0.8f };

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

        bool spawnAsHallucination = ShouldSpawnHallucination();

        GameObject prefab = PickPrefab(spawnAsHallucination);
        if (prefab == null) return;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        // 환각 플래그 설정 — EnemyController가 보상 처리 시 참조
        if (spawnAsHallucination)
        {
            EnemyController ctrl = enemy.GetComponent<EnemyController>();
            if (ctrl != null) ctrl.IsHallucination = true;
        }

        activeEnemies.Add(enemy);
    }

    // 현재 정신력 상태 기반으로 환각 스폰 여부 판단
    private bool ShouldSpawnHallucination()
    {
        if (GameManager.instance == null) return false;

        int stateIndex = (int)GameManager.instance.CurrentMentalState;
        if (stateIndex < 0 || stateIndex >= hallucinationChancePerState.Length) return false;

        float chance = hallucinationChancePerState[stateIndex];
        return Random.value < chance;
    }

    // 환각 여부에 따라 프리팹 선택
    private GameObject PickPrefab(bool hallucination)
    {
        if (hallucination && hallucinationPrefabs != null && hallucinationPrefabs.Length > 0)
            return hallucinationPrefabs[Random.Range(0, hallucinationPrefabs.Length)];

        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
            return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        return null;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 candidate = (Vector2)player.position + randomOffset;

            if (Vector2.Distance(candidate, player.position) < minSpawnDistance)
                continue;

            return candidate;
        }

        return Vector2.zero;
    }

    private void CleanUpDeadEnemies()
    {
        activeEnemies.RemoveAll(e => e == null);
    }
}
