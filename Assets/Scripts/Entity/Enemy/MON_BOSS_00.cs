using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MON_BOSS_00 : EnemyController
{
    HashSet <Collider2D> hitTargets = new HashSet <Collider2D>();

    public GameObject projectilePrefab;
    public float fireRange = 7f;
    public float attackRate = 4f;


    [Header("근접 공격 설정")]
    public float startAttackX = 1f;
    public float endAttackX = 5f;

    public float attackStartDis = 1.5f;
    public float attackDuration = 2f;
    public float attackHeight = 5f;
    public float attackDamage = 10f;

    private float lastFireTime;

    Collider2D detectPlayer;

    protected bool CloseAttack()
    {
        Collider2D detectPlayer = Physics2D.OverlapCircle(transform.position, fireRange, _playerLayer);
        return detectPlayer != null;
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.transform.position - transform.position).normalized;
        projectile.GetComponent<BossProjectile>().Setup(direction);
    }
    private void OnDrawGizmosSelected()
    {
        // Gizmos 색상을 노란색으로 설정
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fireRange);
    }  
    protected override void MonsterAbility()
    {
        if(Time.time >= lastFireTime + attackRate)
        {
            if(CloseAttack())
            {
                Debug.Log(transform.position.x);
                StartCoroutine(SequentialAttack());
                lastFireTime = Time.time;
            }
            else
            {
                FireProjectile();
                lastFireTime = Time.time;
            }

        }
        if(HP / maxHP > 0.5f)
        {
            //몬스터 소환
        }

    }

    IEnumerator SequentialAttack()
    {
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
                if (hit.TryGetComponent<Entity>(out Entity entity))
                {
                    entity.TakeDamage(attackDamage);
                    hitTargets.Add(hit); // 이 공격 한 번에는 다시 안 맞게 추가
                    Debug.Log("플레이어 타격 성공!");
                }
            }
            Debug.DrawRay(hitBox, Vector2.up * attackHeight, Color.red);
            yield return null; 

        }
    }
}
