using NUnit.Framework;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f; // 발사체 속도
    private Vector3 startPos, moveDirection;

    public bool isCharge, backToPlayer;

    public PlayerController playerController;

    [SerializeField]
    private GameObject player;

    private Collider2D collider2D;

    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position; // 시작 위치 저장
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        player = GameObject.FindGameObjectWithTag("Player");
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
            }
        }
        else
        {
            if(Vector3.Distance(startPos, transform.position) <= 12.5f && !backToPlayer) // 일정 거리 이동
            {
                transform.Translate(moveDirection * speed * Time.deltaTime, Space.World); // 발사체 이동 로직
                if(Vector3.Distance(startPos, transform.position) >= 12.5f)
                {
                    backToPlayer = true;
                }
            }
            else if(Vector3.Distance(startPos, transform.position) >= 0 && backToPlayer)
            {
                moveDirection = (player.transform.position - transform.position).normalized;
                rb.linearVelocity = moveDirection * speed;
                if(Vector3.Distance(startPos, transform.position) <= 0)
                {
                    backToPlayer = false;
                }
            }
        }
    }
    
    public void SetDirection(float dir) // BaseAttack에서 방향 받아오는 함수
    {
        moveDirection = new Vector2(dir, 0).normalized; 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Enemy" && !isCharge) 
        {
            rb.simulated = false;
            Destroy(gameObject); //적과 충돌 시 삭제
            
        }
        if(collision.transform.tag == "Enemy" && isCharge)
        {
            playerController.mp += 5;
        }
        if(collision.transform.tag == "Player" && backToPlayer)
        {
            Destroy(gameObject);
        }
        
    }

}
