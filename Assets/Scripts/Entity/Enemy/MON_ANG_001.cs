using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MON_ANG_001 : EnemyController
{
    [SerializeField] private float closeAttackRange;
    [SerializeField] private Transform attackPoint; // 몬스터 앞 공격 중심점 (Empty Object)
    [SerializeField] private GameObject attackHitboxPrefab;  //히트박스 프리팹
    [SerializeField] private float attackRate;

    [Header("공격 타이밍 조절")]
    [SerializeField] private float delayBeforeHitbox = 0.2f; // 선딜레이 (애니메이션에서 무기를 실제로 휘두르는 타이밍)
    [SerializeField] private float totalAttackDuration = 0.5f; // 전체 공격 동작 시간

    private float lastAttackTime = -9999f; 
    

    protected override void MonsterAbility()
    {
        if(isAttacking) return;

        Collider2D detectPlayer = Physics2D.OverlapCircle(transform.position, closeAttackRange, _playerLayer);
        if(detectPlayer != null && (Time.time >= lastAttackTime + attackRate))
        {
            isAttacking = true;
            rb.linearVelocity = Vector2.zero;
            lastAttackTime = Time.time;
            animator.SetTrigger("isAttack");
        }
    }

    public void SpawnHitbox()
    {
        if(attackHitboxPrefab != null && attackPoint != null)
        {
            GameObject hitbox = Instantiate(attackHitboxPrefab, attackPoint.position, Quaternion.identity);

            EnemyHitBox hitboxScript = hitbox.GetComponent<EnemyHitBox>();
            if(hitboxScript != null)
            {
                hitboxScript.damage = this.Attack_Power; // 내 공격력을 담아줌
                hitboxScript.attackOwner = this.transform; // 내 위치를 담아줌 (넉백용)
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

}
