using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

public abstract class EnemyController : Entity
{
    [SerializeField] private float _maxHP;
    [SerializeField] protected float _attackPower;
    [SerializeField] private float _moveSpeed;
    [SerializeField] protected float _detectRange;
    [SerializeField] private float _onDeath_Mental;
    [SerializeField] private float _reward_EXP;

    public GameObject damageTextPrefab; // Inspector에서 프리팹 할당
    public Transform textSpawnPoint;    // 텍스트가 뜰 위치 (예: 몬스터 머리 위 빈 오브젝트)

    // EnemySpawner가 환각 몬스터 스폰 시 true로 설정
    // 환각 몬스터는 보상을 드롭하지 않음 (정신력 패널티는 그대로 적용)
    [HideInInspector] public bool IsHallucination = false;
    private Vector3 direction, moveDirection; //플레이어 따라가기 위한 벡터 값.
    protected SpriteRenderer spriteRenderer;
    protected GameObject player;
    protected PlayerController playerController;
    [SerializeField] protected LayerMask _playerLayer;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected bool isAttacking, inFarAttackRange = false;
    void Awake()
    {
        base.Setup();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        Attack_Power = _attackPower;
        Speed = _moveSpeed;
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (CanUseAbility())
        {
            MonsterAbility();
        }
        if (rb.linearVelocity.normalized.x == 0) // Idle과 Run 애니메이션 제어문
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }
    }

    void FixedUpdate()
    {
        if(isAttacking || inFarAttackRange) return;
        Collider2D detectPlayer = Physics2D.OverlapCircle(transform.position, _detectRange, _playerLayer);
        if(detectPlayer != null){
            if (detectPlayer.CompareTag("Player"))
            {
                // direction = player.transform.position - transform.position;
                // if(direction.y > 0)
                // {
                //     direction.y = 0;
                // }
                // moveDirection = direction.normalized;
                // rb.linearVelocity = moveDirection * _moveSpeed;
                if (gameObject.CompareTag("Boss00"))
                {
                    if(rb.linearVelocityX > 0)
                {
                    transform.localScale = new Vector3(-5, 5, 1);
                }
                else
                {
                    transform.localScale = new Vector3(5, 5, 1);
                }
                }
                else
                {
                    if(rb.linearVelocityX >= 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                }
                
                float directionX = 0f;

                if (player.transform.position.x > transform.position.x)
                {
                    directionX = 1f;  // 플레이어가 오른쪽에 있음
                }
                else if (player.transform.position.x < transform.position.x)
                {
                    directionX = -1f; // 플레이어가 왼쪽에 있음
                }

                // 2. X축은 계산된 이동 속도를 적용하고, Y축은 기존의 중력 낙하 속도를 그대로 보존합니다!
                rb.linearVelocity = new Vector2(directionX * _moveSpeed, rb.linearVelocity.y);
                }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

    }

    public override float maxHP => _maxHP;
    public override float maxMP => 0;
    public override float maxMental => 0f;
    //public override float maxStamina => 0f;


    public override void TakeDamage(float damage) // 데미지 계산
    {
        if(HP > 0)
        {
            HP -= damage;
            // 1. 데미지 텍스트 생성
            // 몬스터 머리 위 위치 기준, 약간의 랜덤성을 주면 글자가 겹치지 않아 더 자연스럽습니다.
            Vector3 spawnPosition = textSpawnPoint.position + new Vector3(Random.Range(-0.2f, 0.2f), 0, 0);
            GameObject textObj = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);

            // 2. 데미지 수치 전달
            DamageText damageText = textObj.GetComponent<DamageText>();
            if(this.CompareTag("Boss00"))
            {
                damageText.SetFontSize(30f);
                damageText.moveSpeed = 8f;
            }
            if (damageText != null)
            {
                damageText.Setup(damage);
            }
        }
        if(HP<=0)
        {
            ReduceMental(); // 죽으면 멘탈 감소
            if (!IsHallucination)
            {
                // TODO: 보상 드롭 처리 (_reward_EXP 등)
            }
            Destroy(gameObject);
        }
        Debug.Log("적 HP : " + HP);

        StartCoroutine("HitAnimation");
    }

    private IEnumerator HitAnimation() 
    {
        Color color = spriteRenderer.color;

        color.a = 0.2f;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(0.2f);

        color.a = 1;
        spriteRenderer.color = color;
    }

    private void ReduceMental()
    {
        // Mathf.Abs로 인스펙터 값의 부호와 관계없이 항상 정신력 감소
        playerController.Mental -= Mathf.Abs(_onDeath_Mental);
    }

        private void OnDrawGizmosSelected()
    {
        // Gizmos 색상을 노란색으로 설정
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectRange);
    }  

    protected virtual bool CanUseAbility()
    {
        Collider2D detectPlayer = Physics2D.OverlapCircle(transform.position, _detectRange, _playerLayer);
        return detectPlayer != null;
    }

    protected abstract void MonsterAbility();
}
