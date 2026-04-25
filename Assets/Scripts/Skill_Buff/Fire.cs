using System.Security.Cryptography;
using NUnit.Framework;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f; // 발사체 속도 (발사 시)
    [SerializeField]
    private float returnSpeed = 20f; // 복귀 속도 (플레이어보다 항상 빠르게 설정)
    private Vector3 startPos, moveDirection;

    public bool isCharge, backToPlayer;

    public float damage;
    public PlayerController playerController;
    public PlayerAttackSystem playerAttackSystem;

    [SerializeField]
    private GameObject player;
    private GameObject AtkSystem;
    private GameObject enemy;
    private GameObject firespirit;

    CircleCollider2D circleCollider2D;
    Rigidbody2D rb;

    void Start()
    {
        startPos = transform.position; // 시작 위치 저장
        rb = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        AtkSystem = GameObject.FindGameObjectWithTag("AtkSystem");
        playerAttackSystem = AtkSystem.GetComponent<PlayerAttackSystem>();
        enemy = GameObject.FindGameObjectWithTag("Enemy");
    }

    public void Setup(GameObject Spirit)
    {
        firespirit = Spirit;   
    }
    // Update is called once per frame
    void Update()
    {
        if(!isCharge)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World); // 발사체 이동 로직
            if(Vector3.Distance(startPos, transform.position) >= 12.5f) // 일정 거리 이동하면 삭제
            {
                Destroy(gameObject);
                playerAttackSystem.baseAttacking = false;
            }
        }
        else
        {
            if(!backToPlayer) // 일정 거리 이동
            {
                transform.Translate(moveDirection * speed * Time.deltaTime, Space.World); // 발사체 이동 로직
                if(Vector3.Distance(startPos, transform.position) >= 12.5f)
                {
                    backToPlayer = true;
                }
            }
            else
            {
                moveDirection = (player.transform.position - transform.position).normalized;
                // 플레이어 현재 이동속도보다 항상 빠르게 보장
                float actualReturnSpeed = Mathf.Max(returnSpeed, playerController.FinalSpeed * 1.5f);
                rb.linearVelocity = moveDirection * actualReturnSpeed;
                if(Vector3.Distance(player.transform.position, transform.position) <= 0.5f)
                {
                    Destroy(gameObject);
                    playerAttackSystem.chargeAttaking = false;
                    
                }
            }
        }
    }
    
    public void SetDirection(Vector2 dir) // PlayerAttackSystem에서 방향 받아오는 함수
    {
        moveDirection = dir.normalized;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            if(!isCharge) 
            {
                collision.GetComponent<Entity>().TakeDamage(damage); // 적에게 데미지
                Destroy(gameObject);
                playerAttackSystem.baseAttacking = false;
            }
            else 
            {
                collision.GetComponent<Entity>().TakeDamage(damage);
                playerController.MP += 5;
                Debug.Log(playerController.MP);
            }
        }
        if(collision.transform.tag == "Player" && backToPlayer && isCharge)
        {
            firespirit.SetActive(true);
            playerAttackSystem.chargeAttaking = false;
            Destroy(gameObject);
            Debug.Log("플레이어와 충돌");
        }
    }
}
