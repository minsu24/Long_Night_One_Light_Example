using UnityEngine;

public class SweepSkill : SkillBase
{   
    public float sweepRadius = 3f; // 휩쓸기 범위
    public float sweepDamage = 15f;

    [SerializeField] private GameObject SSkillFirePrefab; // 발사체 프리팹

    public GameObject enemy;
    public EnemyController enemyController;
    public PlayerController playerController;



    void Awake()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyController = enemy.GetComponent<EnemyController>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

    }
    protected override void OnCast()
    {
        GameObject fire = Instantiate(SSkillFirePrefab, transform.position, Quaternion.identity); // 발사체 생성
        SSkillFire fireScript = fire.GetComponent<SSkillFire>();
        if(fireScript != null)
        {
            fireScript.damage = sweepDamage; // 발사체 데미지 설정
            Vector2 dir = GetFireDirection();
            fireScript.SetDirection(dir);
            if(transform.root.localScale.x == -1 && dir.y == 0)
                fire.GetComponent<SpriteRenderer>().flipX = true;
        }
        // 스킬 사용 법 변경으로 인한 주석처리
        // Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, sweepRadius);
        // foreach (Collider2D enemy in hitEnemies)
        // {
        //     if (enemy.CompareTag("Enemy"))
        //     {
        //         enemy.GetComponent<Entity>().TakeDamage(sweepDamage);
        //         Debug.Log(enemyController.HP);
        //         // 여기에 휩쓸기 이펙트나 사운드 추가!
        //     }
        // }
    }
    private void OnDrawGizmosSelected()
    {
        // Gizmos 색상을 노란색으로 설정
        Gizmos.color = Color.yellow;
    
        // 현재 오브젝트 위치를 중심으로 sweepRadius 크기의 원을 그림
        // 2D 게임이지만 DrawWireSphere를 사용하면 XY 평면에 원형으로 보입니다.
        Gizmos.DrawWireSphere(transform.position, sweepRadius);
    }  

    private Vector2 GetFireDirection()
    {
        if (Input.GetKey(KeyCode.UpArrow)){
            return Vector2.up;
        }

        return new Vector2(transform.root.localScale.x, 0f);
    }
}
