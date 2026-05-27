using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MON_BOSS_00 : EnemyController
{
    HashSet <Collider2D> hitTargets = new HashSet <Collider2D>();

    public GameObject projectilePrefab;
    public float closeRange = 8f;
    public float farRange = 12;
    public float attackRate = 4f;
    public float spawnRate = 2f;


    [Header("근접 공격 설정")]
    public float startAttackX = 1f;
    public float endAttackX = 5f;

    public float attackStartDis = 1.5f;
    public float attackDuration = 2f;
    public float attackHeight = 5f;
    public float attackDamage = 10f;

    [Header("몬스터 스폰 패턴 설정")]
    public GameObject ANGMonsterPrefab;
    public GameObject ADHMonsterPrefab;
    public List<GameObject> nowSpawnMonster;

    private Vector3 SpawnPosition;


    private float lastFireTime = -9999f;
    private float lastSpawnTime;

    private Vector2 currentHitBoxPosForGizmo;
    private Vector2 currentHitSizeForGizmo;


    protected bool CloseAttack()
    {
        Collider2D ClosedetectPlayer = Physics2D.OverlapCircle(transform.position, closeRange, _playerLayer);
        return ClosedetectPlayer != null;
    }

    protected bool FarAttack()
    {
        Collider2D FardetectPlayer = Physics2D.OverlapCircle(transform.position, farRange, _playerLayer);
        return FardetectPlayer != null && !CloseAttack();
    }

    void FireProjectile()
    {
        rb.linearVelocity = Vector2.zero;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.transform.position - transform.position).normalized;
        projectile.GetComponent<BossProjectile>().Setup(direction);
        lastFireTime = Time.time;
    }

    void SpawnMonster()
    {
        // 리스트에서 이미 파괴된(null이 된) 몬스터들을 제거
        // 유니티에서 Destroy된 객체는 리스트에 null로 남기 때문에 이 작업이 필요
        nowSpawnMonster.RemoveAll(m => m == null);

        // 리스트에 남은 몬스터가 있다면 아직 살아있는 것이므로 리턴
        if (nowSpawnMonster.Count > 0)
        {
            return;
        }

        // 모든 몬스터가 죽었으므로 새로 스폰
        for(int i = 0; i < 2; i++)
        {
            GameObject prefab;
            if(i == 0)
            {
                SpawnPosition = new Vector3(18f, -5f, 1);
                prefab = ANGMonsterPrefab;
            }
            else
            {
                SpawnPosition = new Vector3(8f, -11f, 1);
                prefab = ADHMonsterPrefab;
            }
            GameObject monster =  Instantiate(prefab, SpawnPosition, Quaternion.identity);
            nowSpawnMonster.Add(monster);
        }
    }


    private void OnDrawGizmosSelected()
    {
        // Gizmos 색상을 노란색으로 설정
        Gizmos.color = Color.red;
        // 1. 기존 원형 탐지 범위 (반투명하게)
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f); // 빨간색 반투명 원
        Gizmos.DrawSphere(transform.position, closeRange);
        
        Gizmos.color = new Color(0f, 1f, 0f, 0.1f); // 초록색 반투명 원
        Gizmos.DrawSphere(transform.position, farRange);

        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, _detectRange);

        // 2. 근접 공격(OverlapBox) 예상 범위 그리기
        // 게임이 실행 중이지 않을 때도 기획자가 범위를 눈으로 볼 수 있게 합니다.
        float direction = transform.localScale.x > 0 ? -1f : 1f;
        Gizmos.color = Color.yellow;

        // 공격 시작 지점 박스 (노란색 선)
        Vector2 startPos = new Vector2(transform.position.x + (startAttackX * direction), transform.position.y);
        Vector2 defaultSize = new Vector2(Math.Abs(endAttackX - startAttackX), attackHeight);
        Gizmos.DrawWireCube(startPos, defaultSize);

        // 공격 끝 지점 박스 (옅은 노란색 선)
        Vector2 endPos = new Vector2(transform.position.x + (endAttackX * direction), transform.position.y);
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.5f);
        Gizmos.DrawWireCube(endPos, defaultSize);

        // 3. 실제 공격 중일 때 움직이는 히트박스 실시간 시각화 (게임 실행 중 붉은색 채워진 박스)
        if (isAttacking && !inFarAttackRange)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.6f); // 진한 빨간색 사각형
            Gizmos.DrawCube(currentHitBoxPosForGizmo, currentHitSizeForGizmo);
        }
    }  
    protected override void MonsterAbility()
    {
        if(isAttacking) return;
        if(Time.time >= lastFireTime + attackRate)
        {
            if(CloseAttack())
            {
                Debug.Log(transform.position.x);
                spriteRenderer.flipX = true;
                isAttacking = true;
                animator.SetTrigger("CloseAttack"); 
                return;
            }
            else if(FarAttack())
            {
                inFarAttackRange = true;
                isAttacking = true;
                animator.SetTrigger("FarAttack");
                return;
            }
            inFarAttackRange = false;
        }
        if(HP / maxHP < 0.5f && Time.time >= lastSpawnTime + spawnRate)
        {
            SpawnMonster();
            lastSpawnTime = Time.time;
        }

    }

    void StartSequentialAttack()
    {
        StartCoroutine(SequentialAttack());
    }

    IEnumerator SequentialAttack()
    {
        inFarAttackRange = false;
        lastFireTime = Time.time;
        hitTargets.Clear();
        float time = 0f;
        float direction = transform.localScale.x > 0 ? -1f : 1f;
        // 보스 앞 중앙 위치 계산
        Vector2 centerOrigin = (Vector2)transform.position + new Vector2(attackStartDis * direction, 0);
        while(time < attackDuration)
        {
            time += Time.deltaTime;
            float t = time / attackDuration;
            float currentX = Mathf.Lerp(startAttackX, endAttackX, t);
            Vector2 hitBox = new Vector2(transform.position.x + (currentX * direction), transform.position.y);
            Vector2 hitSize = new Vector2(Math.Abs(endAttackX - startAttackX), attackHeight);
            Collider2D hit = Physics2D.OverlapBox(hitBox, hitSize, 0, _playerLayer);

            if (hit != null && !hitTargets.Contains(hit))
            {
                if (hit.TryGetComponent<PlayerController>(out PlayerController playerController))
                {
                    playerController.TakeDamage(attackDamage);
                    float attackdirection = transform.position.x > 0 ? -1f : 1f;
                    playerController.ApplyKnockback(attackdirection);
                    hitTargets.Add(hit); // 이 공격 한 번에는 다시 안 맞게 추가
                    Debug.Log("플레이어 타격 성공!");
                }
            }
            Debug.DrawRay(hitBox, Vector2.up * attackHeight, Color.red);
            yield return null; 
        }

    }

    public void EndAttack()
    {
        isAttacking = false;
        spriteRenderer.flipX = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("SpecialBlock")) // 특정 태그를 가진 블록은 보스 통과
        {
            Collider2D bossCollider = GetComponent<Collider2D>();
            Collider2D blockCollider = collision.collider;

            // 즉시 서로의 충돌을 무시하도록 설정
            Physics2D.IgnoreCollision(bossCollider, blockCollider, true);
        }
    }
}
