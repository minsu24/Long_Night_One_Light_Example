using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MON_ANG_001 : EnemyController
{
    [SerializeField] private float closeAttackRange;
    [SerializeField] private Transform attackPoint; // 몬스터 앞 공격 중심점 (Empty Object)
    [SerializeField] private Vector2 attackSize;     // 공격 범위 크기
    [SerializeField] private LayerMask playerLayer;  // 플레이어 레이어
    
    bool isAttacking = false;



    protected override void MonsterAbility()
    {
        if(isAttacking) return;
        Collider2D detectPlayer = Physics2D.OverlapCircle(transform.position, closeAttackRange, _playerLayer);
        if(detectPlayer != null)
        {
            isAttacking = true;
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(AttackRoutine());
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }
    
    IEnumerator AttackRoutine()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0f, playerLayer);
        animator.SetTrigger("isAttack");
        foreach (Collider2D player in hitPlayers)
        {
            Debug.Log("플레이어 타격!");
            playerController.TakeDamage(_attackPower);
            player.SendMessage("OnCollisionEnter2D", GetComponent<Collider2D>(), SendMessageOptions.DontRequireReceiver);
        }
        yield return new WaitForSeconds(0.5f);
        EndAttack();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}
