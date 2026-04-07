using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class EnemyController : Entity
{
    [SerializeField] private float _maxHP;
    [SerializeField] private float _attackPower;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _detectRange;
    [SerializeField] private float _onDeath_Mental;
    [SerializeField] private float _reward_EXP;
    private Vector3 moveDirection; //플레이어 따라가기 위한 벡터 값.
    SpriteRenderer spriteRenderer;
    private GameObject player;
    private PlayerController playerController;
    [SerializeField] private LayerMask _playerLayer;
    Rigidbody2D rb;
    void Awake()
    {
        base.Setup();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        Attack_Power = _attackPower;
        Speed = _moveSpeed;
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        
    }
    void FixedUpdate()
    {
        Collider2D detectPlayer = Physics2D.OverlapCircle(transform.position, _detectRange, _playerLayer);
        if(detectPlayer != null){
            if (detectPlayer.CompareTag("Player"))
            {
                moveDirection = (player.transform.position - transform.position).normalized;
                rb.linearVelocity = moveDirection * _moveSpeed;
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
        playerController.Mental -= _onDeath_Mental;
    }

        private void OnDrawGizmosSelected()
    {
        // Gizmos 색상을 노란색으로 설정
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectRange);
    }  
}
