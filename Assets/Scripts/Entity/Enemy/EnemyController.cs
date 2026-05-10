using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public abstract class EnemyController : Entity
{
    [SerializeField] private float _maxHP;
    [SerializeField] private float _attackPower;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _detectRange;
    [SerializeField] private float _onDeath_Mental;
    [SerializeField] private float _reward_EXP;

    // EnemySpawner가 환각 몬스터 스폰 시 true로 설정
    // 환각 몬스터는 보상을 드롭하지 않음 (정신력 패널티는 그대로 적용)
    [HideInInspector] public bool IsHallucination = false;
    private Vector3 moveDirection; //플레이어 따라가기 위한 벡터 값.
    SpriteRenderer spriteRenderer;
    protected GameObject player;
    protected PlayerController playerController;
    [SerializeField] protected LayerMask _playerLayer;
    protected Rigidbody2D rb;
    protected Animator animator;
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
        Collider2D detectPlayer = Physics2D.OverlapCircle(transform.position, _detectRange, _playerLayer);
        if(detectPlayer != null){
            if (detectPlayer.CompareTag("Player"))
            {
                moveDirection = (player.transform.position - transform.position).normalized;
                rb.linearVelocity = moveDirection * _moveSpeed;
                if(rb.linearVelocityX >= 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, 0);
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
