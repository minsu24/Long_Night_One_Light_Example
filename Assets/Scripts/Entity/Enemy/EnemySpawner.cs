using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("일반 스폰 설정")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int maxEnemyCount = 10;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float minSpawnX = 10f;
    [SerializeField] private float maxSpawnX = 10f;

    [SerializeField] private float minSpawnTime = 4f;

    [Header("환각 몬스터 설정")]
    [SerializeField] private GameObject[] hallucinationPrefabs;  // 환각 전용 프리팹 (없으면 일반 프리팹 사용)

    // 정신력 단계별 환각 스폰 확률: [Normal, Depression, Anxiety, Collapse]
    // 붕괴 구간으로 갈수록 환각 빈도가 점진적으로 증가
    // Normal=0%, Depression=0%, Anxiety=50%, Collapse=80% — 불안 단계부터 환각 스폰
    [SerializeField] private float[] hallucinationChancePerState = { 0f, 0f, 0.5f, 0.8f };

    private Transform player;
    private List<GameObject> activeEnemies = new List<GameObject>();

    bool firstSpawn = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // yield return new WaitForSeconds(spawnInterval);
            // CleanUpDeadEnemies();
            // Debug.Log("현재 적의 수 : " + activeEnemies.Count);
            
            // if (activeEnemies.Count < maxEnemyCount){
            //     Debug.Log(firstSpawn);
            //     if(activeEnemies.Count == 0 && !firstSpawn)
            //     {
            //         yield return new WaitForSeconds(minSpawnTime);
            //         SpawnEnemy();
            //     }
            //     else
            //     {
            //         SpawnEnemy();
            //         firstSpawn = false;
            //     }
                
            // }
                // [1단계: 스폰] maxEnemyCount에 도달할 때까지 몬스터를 소환합니다.
            while (activeEnemies.Count < maxEnemyCount)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval); // spawnInterval 간격으로 1마리씩 소환
                CleanUpDeadEnemies(); // 소환하는 도중에 적이 죽을 수도 있으므로 리스트 정리
            }

            // [2단계: 전투 대기] 몬스터가 모두 죽을 때까지 더 이상 소환하지 않고 기다립니다.
            while (activeEnemies.Count > 0)
            {
                yield return new WaitForSeconds(0.5f); // 0.5초마다 적이 전멸했는지 확인 (성능 최적화)
                CleanUpDeadEnemies(); // 죽은 몬스터를 리스트에서 제거하여 Count를 줄임
            }

            // [3단계: 휴식] 몬스터가 0마리가 되면 루프를 빠져나와 휴식 시간을 가집니다.
            Debug.Log($"웨이브 클리어! {minSpawnTime}초 뒤에 다음 스폰을 시작합니다.");
            yield return new WaitForSeconds(minSpawnTime);
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
        for (int i = 0; i < maxEnemyCount; i++)
        {
            
            // 2. 최소 거리 ~ 최대 거리 사이의 무작위 X 오프셋 계산
            float randomXOffset = Random.Range(minSpawnX, maxSpawnX);
            
            // 3. 플레이어 위치를 기반으로 최종 좌표 생성
            float spawnX = transform.position.x + randomXOffset;
            float spawnY = transform.position.y; // Y값은 플레이어 기준으로 항상 일정하게 고정

            Vector2 candidate = new Vector2(spawnX, spawnY);

            // 이전 원형 스폰과 달리 이미 X축 최소 거리가 보장되므로 Distance 체크는 생략해도 안전합니다.
            return candidate;
        }
        
        return Vector2.zero;
    }

    private void CleanUpDeadEnemies()
    {
        activeEnemies.RemoveAll(e => e == null);
    }
}
