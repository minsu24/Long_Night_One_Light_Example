using UnityEngine;

public class SweepSkill : SkillBase
{   
    public float sweepRadius = 3f; // 휩쓸기 범위
    public float sweepDamage = 15f;


    public GameObject enemy;
    public EnemyController enemyController;

    void Awake()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyController = enemy.GetComponent<EnemyController>();
    }
    protected override void OnCast()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, sweepRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Entity>().TakeDamage(sweepDamage);
                Debug.Log(enemyController.HP);
                // 여기에 휩쓸기 이펙트나 사운드 추가!
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Gizmos 색상을 노란색으로 설정
        Gizmos.color = Color.yellow;
    
        // 현재 오브젝트 위치를 중심으로 sweepRadius 크기의 원을 그림
        // 2D 게임이지만 DrawWireSphere를 사용하면 XY 평면에 원형으로 보입니다.
        Gizmos.DrawWireSphere(transform.position, sweepRadius);
    }   
}
